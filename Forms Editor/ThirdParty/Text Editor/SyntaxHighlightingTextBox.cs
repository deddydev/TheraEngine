using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Windows.Forms;
using System.Text;
using System.Runtime.InteropServices;
using Core.Win32.Native;

namespace TheraEditor.Core.SyntaxHighlightingTextBox
{
	/// <summary>
	/// A textbox the does syntax highlighting.
	/// </summary>
	public class SyntaxHighlightingTextBox : RichTextBox 
	{
		#region Members
        
		//Internal use members
		private bool _autoCompleteShown = false;
		private bool _parsing = false;
		private bool _ignoreLostFocus = false;

		private AutoCompleteForm _autoCompleteForm = new AutoCompleteForm();

		//Undo/Redo members
		private ArrayList _undoList = new ArrayList();
		private Stack _redoStack = new Stack();
		private bool _isUndo = false;
		private UndoRedoInfo _lastInfo = new UndoRedoInfo("", new POINT(), 0);

		#endregion

		#region Properties
		/// <summary>
		/// Determines if token recognition is case sensitive.
		/// </summary>
		[Category("Behavior")]
        public bool CaseSensitive { get; set; }
		/// <summary>
		/// Sets whether or not to remove items from the Autocomplete window as the user types...
		/// </summary>
		[Category("Behavior")]
        public bool FilterAutoComplete { get; set; }
		/// <summary>
		/// Set the maximum amount of Undo/Redo steps.
		/// </summary>
		[Category("Behavior")]
        public int MaxUndoRedoSteps { get; set; }

		/// <summary>
		/// A collection of characters. a token is every string between two seperators.
		/// </summary>
		public SeparatorCollection Separators { get; }
            = new SeparatorCollection();
		/// <summary>
		/// The collection of highlight descriptors.
		/// </summary>
		/// 
		public HighlightDescriptorCollection HighlightDescriptors { get; }
            = new HighlightDescriptorCollection();

		#endregion

		#region Overridden methods

		/// <summary>
		/// The on text changed overrided. Here we parse the text into RTF for the highlighting.
		/// </summary>
		/// <param name="e"></param>
		protected override void OnTextChanged(EventArgs e)
		{
			if (_parsing)
                return;
			_parsing = true;
			NativeMethods.LockWindowUpdate(Handle);
			base.OnTextChanged(e);

			if (!_isUndo)
			{
				_redoStack.Clear();
				_undoList.Insert(0, _lastInfo);
				LimitUndo();
				_lastInfo = new UndoRedoInfo(Text, GetScrollPos(), SelectionStart);
			}
			
			//Save scroll bar an cursor position, changeing the RTF moves the cursor and scrollbars to top positin
			POINT scrollPos = GetScrollPos();
			int cursorLoc = SelectionStart;

			//Created with an estimate of how big the stringbuilder has to be...
			StringBuilder sb = new StringBuilder((int)(Text.Length * 1.5 + 150));

			//Adding RTF header
			sb.Append(@"{\rtf1\fbidis\ansi\ansicpg1255\deff0\deflang1037{\fonttbl{");
			
			//Font table creation
			int fontCounter = 0;
			Hashtable fonts = new Hashtable();
			AddFontToTable(sb, Font, ref fontCounter, fonts);
			foreach (HighlightDescriptor hd in HighlightDescriptors)
				if ((hd.Font !=  null) && !fonts.ContainsKey(hd.Font.Name))
					AddFontToTable(sb, hd.Font,ref fontCounter, fonts);
			
			sb.Append("}\n");

			//ColorTable
			
			sb.Append(@"{\colortbl ;");
			Hashtable colors = new Hashtable();
			int colorCounter = 1;
			AddColorToTable(sb, ForeColor, ref colorCounter, colors);
			AddColorToTable(sb, BackColor, ref colorCounter, colors);
			
			foreach (HighlightDescriptor hd in HighlightDescriptors)
				if (!colors.ContainsKey(hd.Color))
					AddColorToTable(sb, hd.Color, ref colorCounter, colors);

			//Parsing text
			sb.Append("}\n").Append(@"\viewkind4\uc1\pard\ltrpar");
			SetDefaultSettings(sb, colors, fonts);

			char[] sperators = Separators.GetAsCharArray();

			//Replacing "\" to "\\" for RTF...
			string[] lines = Text.Replace("\\","\\\\").Replace("{", "\\{").Replace("}", "\\}").Split('\n');
			for (int lineCounter = 0 ; lineCounter < lines.Length; lineCounter++)
			{
				if (lineCounter != 0)
					AddNewLine(sb);
				
				string line = lines[lineCounter];
				string[] tokens = CaseSensitive ? line.Split(sperators) : line.ToUpper().Split(sperators);
				if (tokens.Length == 0)
				{
					sb.Append(line);
					AddNewLine(sb);
					continue;
				}

				int tokenCounter = 0;
				for (int i = 0; i < line.Length ;)
				{
					char curChar = line[i];
					if (Separators.Contains(curChar))
					{
						sb.Append(curChar);
						++i;
					}
					else
					{
						string curToken = tokens[tokenCounter++];
						bool bAddToken = true;
						foreach	(HighlightDescriptor hd in HighlightDescriptors)
						{
							string compareStr = CaseSensitive ? hd.Token : hd.Token.ToUpper();
							bool match = false;

							//Check if the highlight descriptor matches the current toker according to the DescriptoRecognision property.
							switch (hd.DescriptorRecognition)
							{
								case DescriptorRecognition.WholeWord:
									if (curToken == compareStr)
                                        match = true;
									break;
								case DescriptorRecognition.StartsWith:
									if (curToken.StartsWith(compareStr))
										match = true;
									break;
								case DescriptorRecognition.Contains:
									if (curToken.IndexOf(compareStr) != -1)
										match = true;
									break;
							}
							if (!match)
							{
								//If this token doesn't match chech the next one.
								continue;
							}

							//printing this token will be handled by the inner code, don't apply default settings...
							bAddToken = false;

							//Set colors to current descriptor settings.
							SetDescriptorSettings(sb, hd, colors, fonts);

							//Print text affected by this descriptor.
							switch (hd.DescriptorType)
							{
								case DescriptorType.Word:
									sb.Append(line.Substring(i, curToken.Length));
									SetDefaultSettings(sb, colors, fonts);
									i += curToken.Length;
									break;
								case DescriptorType.ToEOL:
									sb.Append(line.Remove(0, i));
									i = line.Length;
									SetDefaultSettings(sb, colors, fonts);
									break;
								case DescriptorType.ToCloseToken:
									while((line.IndexOf(hd.CloseToken, i) == -1) && (lineCounter < lines.Length))
									{
										sb.Append(line.Remove(0, i));
										lineCounter++;
										if (lineCounter < lines.Length)
										{
											AddNewLine(sb);
											line = lines[lineCounter];
											i = 0;
										}
										else
										{
											i = line.Length;
										}
									}
									if (line.IndexOf(hd.CloseToken, i) != -1)
									{
										sb.Append(line.Substring(i, line.IndexOf(hd.CloseToken, i) + hd.CloseToken.Length - i) );
										line = line.Remove(0, line.IndexOf(hd.CloseToken, i) + hd.CloseToken.Length);
										tokenCounter = 0;
										tokens = CaseSensitive ? line.Split(sperators) : line.ToUpper().Split(sperators);
										SetDefaultSettings(sb, colors, fonts);
										i = 0;
									}
									break;
							}
							break;
						}
						if (bAddToken)
						{
							//Print text with default settings...
							sb.Append(line.Substring(i, curToken.Length));
							i+=	curToken.Length;
						}
					}
				}
			}
	
            //System.Diagnostics.Debug.WriteLine(sb.ToString());
			Rtf = sb.ToString();

			//Restore cursor and scrollbars location.
			SelectionStart = cursorLoc;

			_parsing = false;

			SetScrollPos(scrollPos);
			NativeMethods.LockWindowUpdate((IntPtr)0);
			Invalidate();
			
			if (_autoCompleteShown)
			{
				if (FilterAutoComplete)
				{
					SetAutoCompleteItems();
					SetAutoCompleteSize();
					SetAutoCompleteLocation(false);
				}
				SetBestSelectedAutoCompleteItem();
			}
		}

		protected override void OnVScroll(EventArgs e)
		{
			if (_parsing)
                return;
			base.OnVScroll (e);
		}

		protected override void OnMouseDown(MouseEventArgs e)
		{
			HideAutoCompleteForm();
			base.OnMouseDown (e);
		}

		/// <summary>
		/// Taking care of Keyboard events
		/// </summary>
		/// <param name="m"></param>
		/// <remarks>
		/// Since even when overriding the OnKeyDown methoed and not calling the base function 
		/// you don't have full control of the input, I've decided to catch windows messages to handle them.
		/// </remarks>
		protected override void WndProc(ref Message m)
		{
			switch (m.Msg)
			{
				case NativeConstants.WM_PAINT:
				{
					//Don't draw the control while parsing to avoid flicker.
					if (_parsing)
					{
						return;
					}
					break;
				}
				case NativeConstants.WM_KEYDOWN:
				{
					if (_autoCompleteShown)
					{
						switch ((Keys)(int)m.WParam)
						{
							case Keys.Down:
							{
								if (_autoCompleteForm.Items.Count != 0)
								{
									_autoCompleteForm.SelectedIndex = (_autoCompleteForm.SelectedIndex + 1) % _autoCompleteForm.Items.Count;
								}
								return;
							}
							case Keys.Up:
							{
								if (_autoCompleteForm.Items.Count != 0)
								{
									if (_autoCompleteForm.SelectedIndex < 1)
									{
										_autoCompleteForm.SelectedIndex = _autoCompleteForm.Items.Count - 1;
									}
									else
									{
										_autoCompleteForm.SelectedIndex--;
									}
								}
								return;
							}
							case Keys.Enter:
							case Keys.Space:
							{
								AcceptAutoCompleteItem();
								return;
							}
							case Keys.Escape:
							{
								HideAutoCompleteForm();
								return;
							}
								
						}
					}
					else
					{
						if (((Keys)(int)m.WParam == Keys.Space) && 
							((NativeMethods.GetKeyState(NativeConstants.VK_CONTROL) & NativeConstants.KS_KEYDOWN) != 0))
						{
							CompleteWord();
						} 
						else if (((Keys)(int)m.WParam == Keys.Z) && 
							((NativeMethods.GetKeyState(NativeConstants.VK_CONTROL) & NativeConstants.KS_KEYDOWN) != 0))
						{
							Undo();
							return;
						}
						else if (((Keys)(int)m.WParam == Keys.Y) && 
							((NativeMethods.GetKeyState(NativeConstants.VK_CONTROL) & NativeConstants.KS_KEYDOWN) != 0))
						{
							Redo();
							return;
						}
					}
					break;
				}
				case NativeConstants.WM_CHAR:
				{
					switch ((Keys)(int)m.WParam)
					{
						case Keys.Space:
							if ((NativeMethods.GetKeyState(NativeConstants.VK_CONTROL) & NativeConstants.KS_KEYDOWN )!= 0)
							{
								return;
							}
							break;
						case Keys.Enter:
							if (_autoCompleteShown) return;
							break;
					}
				}
				break;

			}
			base.WndProc (ref m);
		}


		/// <summary>
		/// Hides the AutoComplete form when losing focus on textbox.
		/// </summary>
		/// <param name="e"></param>
		protected override void OnLostFocus(EventArgs e)
		{
			if (!_ignoreLostFocus)
			{
				HideAutoCompleteForm();
			}
			base.OnLostFocus (e);
		}


		#endregion

		#region Undo/Redo Code
		public new bool CanUndo 
		{
			get 
			{
				return _undoList.Count > 0;
			}
		}
		public new bool CanRedo
		{
			get 
			{
				return _redoStack.Count > 0;
			}
		}

		private void LimitUndo()
		{
			while (_undoList.Count > MaxUndoRedoSteps)
			{
				_undoList.RemoveAt(MaxUndoRedoSteps);
			}
		}

		public new void Undo()
		{
			if (!CanUndo)
				return;
			_isUndo = true;
			_redoStack.Push(new UndoRedoInfo(Text, GetScrollPos(), SelectionStart));
			UndoRedoInfo info = (UndoRedoInfo)_undoList[0];
			_undoList.RemoveAt(0);
			Text = info.Text;
			SelectionStart = info.CursorLocation;
			SetScrollPos(info.ScrollPos);
			_lastInfo = info;
			_isUndo = false;
		}
		public new void Redo()
		{
			if (!CanRedo)
				return;
			_isUndo = true;
			_undoList.Insert(0,new UndoRedoInfo(Text, GetScrollPos(), SelectionStart));
			LimitUndo();
			UndoRedoInfo info = (UndoRedoInfo)_redoStack.Pop();
			Text = info.Text;
			SelectionStart = info.CursorLocation;
			SetScrollPos(info.ScrollPos);
			_isUndo = false;
		}

		private class UndoRedoInfo
		{
			public UndoRedoInfo(string text, POINT scrollPos, int cursorLoc)
			{
				Text = text;
				ScrollPos = scrollPos;
				CursorLocation = cursorLoc;
			}
			public readonly POINT ScrollPos;
			public readonly int CursorLocation;
			public readonly string Text;
		}
		#endregion

		#region AutoComplete functions

		/// <summary>
		/// Entry point to autocomplete mechanism.
		/// Tries to complete the current word. if it fails it shows the AutoComplete form.
		/// </summary>
		private void CompleteWord()
		{
			int curTokenStartIndex = Text.LastIndexOfAny(Separators.GetAsCharArray(), Math.Min(SelectionStart, Text.Length - 1))+1;
			int curTokenEndIndex= Text.IndexOfAny(Separators.GetAsCharArray(), SelectionStart);
			if (curTokenEndIndex == -1) 
			{
				curTokenEndIndex = Text.Length;
			}
			string curTokenString = Text.Substring(curTokenStartIndex, Math.Max(curTokenEndIndex - curTokenStartIndex,0)).ToUpper();
			
			string token = null;
			foreach (HighlightDescriptor hd in HighlightDescriptors)
			{
				if (hd.UseForAutoComplete && hd.Token.ToUpper().StartsWith(curTokenString))
				{
					if (token == null)
					{
						token = hd.Token;
					}
					else
					{
						token = null;
						break;
					}
				}
			}
			if (token == null)
			{
				ShowAutoComplete();
			}
			else
			{
				SelectionStart = curTokenStartIndex;
				SelectionLength = curTokenEndIndex - curTokenStartIndex;
				SelectedText = token;
				SelectionStart = SelectionStart + SelectionLength;
				SelectionLength = 0;
			}
		}

		/// <summary>
		/// replace the current word of the cursor with the one from the AutoComplete form and closes it.
		/// </summary>
		/// <returns>If the operation was succesful</returns>
		private bool AcceptAutoCompleteItem()
		{
			
			if (_autoCompleteForm.SelectedItem == null)
			{
				return false;
			}
			
			int curTokenStartIndex = Text.LastIndexOfAny(Separators.GetAsCharArray(), Math.Min(SelectionStart, Text.Length - 1)) + 1;
			int curTokenEndIndex= Text.IndexOfAny(Separators.GetAsCharArray(), SelectionStart);
			if (curTokenEndIndex == -1) 
			{
				curTokenEndIndex = Text.Length;
			}
			SelectionStart = Math.Max(curTokenStartIndex, 0);
			SelectionLength = Math.Max(0,curTokenEndIndex - curTokenStartIndex);
			SelectedText = _autoCompleteForm.SelectedItem;
			SelectionStart = SelectionStart + SelectionLength;
			SelectionLength = 0;
			
			HideAutoCompleteForm();
			return true;
		}



		/// <summary>
		/// Finds the and sets the best matching token as the selected item in the AutoCompleteForm.
		/// </summary>
		private void SetBestSelectedAutoCompleteItem()
		{
			int curTokenStartIndex = Text.LastIndexOfAny(Separators.GetAsCharArray(), Math.Min(SelectionStart, Text.Length - 1))+1;
			int curTokenEndIndex= Text.IndexOfAny(Separators.GetAsCharArray(), SelectionStart);
			if (curTokenEndIndex == -1) 
			{
				curTokenEndIndex = Text.Length;
			}
			string curTokenString = Text.Substring(curTokenStartIndex, Math.Max(curTokenEndIndex - curTokenStartIndex,0)).ToUpper();
			
			if ((_autoCompleteForm.SelectedItem != null) && 
				_autoCompleteForm.SelectedItem.ToUpper().StartsWith(curTokenString))
			{
				return;
			}

			int matchingChars = -1;
			string bestMatchingToken = null;

			foreach (string item in _autoCompleteForm.Items)
			{
				bool isWholeItemMatching = true;
				for (int i = 0 ; i < Math.Min(item.Length, curTokenString.Length); i++)
				{
					if (char.ToUpper(item[i]) != char.ToUpper(curTokenString[i]))
					{
						isWholeItemMatching = false;
						if (i-1 > matchingChars)
						{
							matchingChars = i;
							bestMatchingToken = item;
							break;
						}
					}
				}
				if (isWholeItemMatching &&
					(Math.Min(item.Length, curTokenString.Length) > matchingChars))
				{
					matchingChars = Math.Min(item.Length, curTokenString.Length);
					bestMatchingToken = item;
				}
			}
			
			if (bestMatchingToken != null)
			{
				_autoCompleteForm.SelectedIndex = _autoCompleteForm.Items.IndexOf(bestMatchingToken);
			}


		}

		/// <summary>
		/// Sets the items for the AutoComplete form.
		/// </summary>
		private void SetAutoCompleteItems()
		{
			_autoCompleteForm.Items.Clear();
			string filterString = "";
			if (FilterAutoComplete)
			{
				int filterTokenStartIndex = Text.LastIndexOfAny(Separators.GetAsCharArray(), Math.Min(SelectionStart, Text.Length - 1))+1;
				int filterTokenEndIndex= Text.IndexOfAny(Separators.GetAsCharArray(), SelectionStart);
				if (filterTokenEndIndex == -1) 
				{
					filterTokenEndIndex = Text.Length;
				}
				filterString = Text.Substring(filterTokenStartIndex, filterTokenEndIndex - filterTokenStartIndex).ToUpper();
			}

			foreach (HighlightDescriptor hd in HighlightDescriptors)
				if (hd.Token.ToUpper().StartsWith(filterString) && hd.UseForAutoComplete)
					_autoCompleteForm.Items.Add(hd.Token);
			
			_autoCompleteForm.UpdateView();
		}
		
		/// <summary>
		/// Sets the size. the size is limited by the MaxSize property in the form itself.
		/// </summary>
		private void SetAutoCompleteSize()
		{
			_autoCompleteForm.Height = Math.Min(
				Math.Max(_autoCompleteForm.Items.Count, 1) * _autoCompleteForm.ItemHeight + 4, 
				_autoCompleteForm.MaximumSize.Height);
		}

		/// <summary>
		/// closes the AutoCompleteForm.
		/// </summary>
		private void HideAutoCompleteForm()
		{
			_autoCompleteForm.Visible = false;
			_autoCompleteShown = false;
		}
		
		/// <summary>
		/// Sets the location of the AutoComplete form, maiking sure it's on the screen where the cursor is.
		/// </summary>
		/// <param name="moveHorizontly">determines wheather or not to move the form horizontly.</param>
		private void SetAutoCompleteLocation(bool moveHorizontly)
		{
			Point cursorLocation = GetPositionFromCharIndex(SelectionStart);
			Screen screen = Screen.FromPoint(cursorLocation);
			Point optimalLocation = new Point(PointToScreen(cursorLocation).X-15, (int)(PointToScreen(cursorLocation).Y + Font.Size*2 + 2));
			Rectangle desiredPlace = new Rectangle(optimalLocation , _autoCompleteForm.Size);
			desiredPlace.Width = 152;
			if (desiredPlace.Left < screen.Bounds.Left) 
			{
				desiredPlace.X = screen.Bounds.Left;
			}
			if (desiredPlace.Right > screen.Bounds.Right)
			{
				desiredPlace.X -= (desiredPlace.Right - screen.Bounds.Right);
			}
			if (desiredPlace.Bottom > screen.Bounds.Bottom)
			{
				desiredPlace.Y = cursorLocation.Y - 2 - desiredPlace.Height;
			}
			if (!moveHorizontly)
			{
				desiredPlace.X = _autoCompleteForm.Left;
			}

			_autoCompleteForm.Bounds = desiredPlace;
		}

		/// <summary>
		/// Shows the Autocomplete form.
		/// </summary>
		public void ShowAutoComplete()
		{
			SetAutoCompleteItems();
			SetAutoCompleteSize();
			SetAutoCompleteLocation(true);
			_ignoreLostFocus = true;
			_autoCompleteForm.Visible = true;
			SetBestSelectedAutoCompleteItem();
			_autoCompleteShown = true;
			Focus();
			_ignoreLostFocus = false;
		}

		#endregion 

		#region Rtf building helper functions

		/// <summary>
		/// Set color and font to default control settings.
		/// </summary>
		/// <param name="sb">the string builder building the RTF</param>
		/// <param name="colors">colors hashtable</param>
		/// <param name="fonts">fonts hashtable</param>
		private void SetDefaultSettings(StringBuilder sb, Hashtable colors, Hashtable fonts)
		{
			SetColor(sb, ForeColor, colors);
			SetFont(sb, Font, fonts);
			SetFontSize(sb, (int)Font.Size);
			EndTags(sb);
		}

		/// <summary>
		/// Set Color and font to a highlight descriptor settings.
		/// </summary>
		/// <param name="sb">the string builder building the RTF</param>
		/// <param name="hd">the HighlightDescriptor with the font and color settings to apply.</param>
		/// <param name="colors">colors hashtable</param>
		/// <param name="fonts">fonts hashtable</param>
		private void SetDescriptorSettings(StringBuilder sb, HighlightDescriptor hd, Hashtable colors, Hashtable fonts)
		{
			SetColor(sb, hd.Color, colors);
			if (hd.Font != null)
			{
				SetFont(sb, hd.Font, fonts);
				SetFontSize(sb, (int)hd.Font.Size);
			}
			EndTags(sb);

		}
		/// <summary>
		/// Sets the color to the specified color
		/// </summary>
		private void SetColor(StringBuilder sb, Color color, Hashtable colors)
		{
			sb.Append(@"\cf").Append(colors[color]);
		}
		/// <summary>
		/// Sets the backgroung color to the specified color.
		/// </summary>
		private void SetBackColor(StringBuilder sb, Color color, Hashtable colors)
		{
			sb.Append(@"\cb").Append(colors[color]);
		}
		/// <summary>
		/// Sets the font to the specified font.
		/// </summary>
		private void SetFont(StringBuilder sb, Font font, Hashtable fonts)
		{
			if (font == null) return;
			sb.Append(@"\f").Append(fonts[font.Name]);
		}
		/// <summary>
		/// Sets the font size to the specified font size.
		/// </summary>
		private void SetFontSize(StringBuilder sb, int size)
		{
			sb.Append(@"\fs").Append(size*2);
		}
		/// <summary>
		/// Adds a newLine mark to the RTF.
		/// </summary>
		private void AddNewLine(StringBuilder sb)
		{
			sb.Append("\\par\n");
		}

		/// <summary>
		/// Ends a RTF tags section.
		/// </summary>
		private void EndTags(StringBuilder sb)
		{
			sb.Append(' ');
		}

		/// <summary>
		/// Adds a font to the RTF's font table and to the fonts hashtable.
		/// </summary>
		/// <param name="sb">The RTF's string builder</param>
		/// <param name="font">the Font to add</param>
		/// <param name="counter">a counter, containing the amount of fonts in the table</param>
		/// <param name="fonts">an hashtable. the key is the font's name. the value is it's index in the table</param>
		private void AddFontToTable(StringBuilder sb, Font font, ref int counter, Hashtable fonts)
		{
	
			sb.Append(@"\f").Append(counter).Append(@"\fnil\fcharset0").Append(font.Name).Append(";}");
			fonts.Add(font.Name, counter++);
		}

		/// <summary>
		/// Adds a color to the RTF's color table and to the colors hashtable.
		/// </summary>
		/// <param name="sb">The RTF's string builder</param>
		/// <param name="color">the color to add</param>
		/// <param name="counter">a counter, containing the amount of colors in the table</param>
		/// <param name="colors">an hashtable. the key is the color. the value is it's index in the table</param>
		private void AddColorToTable(StringBuilder sb, Color color, ref int counter, Hashtable colors)
		{
	
			sb.Append(@"\red").Append(color.R).Append(@"\green").Append(color.G).Append(@"\blue")
				.Append(color.B).Append(";");
			colors.Add(color, counter++);
		}

		#endregion

		#region Scrollbar positions functions
		/// <summary>
		/// Sends a win32 message to get the scrollbars' position.
		/// </summary>
		/// <returns>a POINT structore containing horizontal and vertical scrollbar position.</returns>
		private unsafe POINT GetScrollPos()
		{
			POINT res = new POINT();
			IntPtr ptr = new IntPtr(&res);
			NativeMethods.SendMessage(Handle, NativeConstants.EM_GETSCROLLPOS, 0, ptr);
			return res;
		}

		/// <summary>
		/// Sends a win32 message to set scrollbars position.
		/// </summary>
		/// <param name="point">a POINT conatining H/Vscrollbar scrollpos.</param>
		private unsafe void SetScrollPos(POINT point)
		{
			IntPtr ptr = new IntPtr(&point);
			NativeMethods.SendMessage(Handle, NativeConstants.EM_SETSCROLLPOS, 0, ptr);
		}
		#endregion
	}
}