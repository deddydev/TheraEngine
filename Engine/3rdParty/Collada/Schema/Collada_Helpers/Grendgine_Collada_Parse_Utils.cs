using System;
using System.Diagnostics;
using TheraEngine;

namespace grendgine_collada
{
	public class Grendgine_Collada_Parse_Utils
	{
		public static int[] String_To_Int(string int_array)
		{
			string[] str = int_array.Split(' ');
			int[] array = new int[str.GetLongLength(0)];
			try
			{
				for (long i = 0; i < str.GetLongLength(0); i++)
				{
					array[i] = Convert.ToInt32(str[i]);
				}
			}
			catch (Exception e)
			{
                Engine.PrintLine(e.ToString());
                Engine.PrintLine("");
                Engine.PrintLine(int_array);
			}
			return array;
		}
		
		public static float[] String_To_Float(string float_array)
		{
			string[] str = float_array.Split(' ');
			float[] array = new float[str.GetLongLength(0)];
			try
			{
				for (long i = 0; i < str.GetLongLength(0); i++)
				{
					array[i] = Convert.ToSingle(str[i]);
				}
			}
			catch (Exception e)
			{
                Engine.PrintLine(e.ToString());
                Engine.PrintLine("");
                Engine.PrintLine(float_array);
			}
			return array;
		}
	
		public static bool[] String_To_Bool(string bool_array)
		{
			string[] str = bool_array.Split(' ');
			bool[] array = new bool[str.GetLongLength(0)];
			try
			{
				for (long i = 0; i < str.GetLongLength(0); i++)
				{
					array[i] = Convert.ToBoolean(str[i]);
				}
			}
			catch (Exception e)
			{
                Engine.PrintLine(e.ToString());
                Engine.PrintLine("");
                Engine.PrintLine(bool_array);
			}
			return array;
		}
		

		
	}
}