using System.Drawing;
using System.Windows.Forms;
using TheraEngine;
using WeifenLuo.WinFormsUI.Docking;

namespace TheraEditor.Windows.Forms
{
    public partial class DockableWelcomeWindow : DockContent
    {
        public DockableWelcomeWindow()
        {
            InitializeComponent();
            label2.Font = Engine.MakeFont("origicide", 20.0f, FontStyle.Regular);
            const string page = @"
<html>
<head>
    <title></title>
    <meta http-equiv='X-UA-Compatible' content='IE=11'/>
    <script src=""https://ajax.googleapis.com/ajax/libs/jquery/2.1.1/jquery.min.js""></script>
    <script>
        var channelID = ""UCekkiovQ0NxqzraXseicDPQ"";
        $.getJSON('https://api.rss2json.com/v1/api.json?rss_url=https%3A%2F%2Fwww.youtube.com%2Ffeeds%2Fvideos.xml%3Fchannel_id%3D' + channelID, function(data) 
        {
            var link = data.items[0].link;
            var id = link.substr(link.indexOf(""="") + 1);
            $(""#youtube_video"").attr(""src"",""https://youtube.com/embed/"" + id + ""?controls=0&showinfo=0&rel=0&autoplay=1"");
        });
    </script>
    <style>
        body
        {
            margin: 0;
            padding: 0;
            background-color: #555555;
        }
        .video_container
        {
	        position:relative;
	        padding-bottom:56.25%;
	        padding-top:30px;
	        height:0;
	        overflow:hidden;
        }
        .video_container iframe, .video_container object, .video_container embed
        {
	        position:absolute;
	        top:0;
	        left:0;
	        width:100%;
	        height:100%;
        }
        #youtube_video
        {
            width: 100%;
        }
    </style>
    </head>
    <body>
        <div class=""video_container"">
            <iframe id=""youtube_video"" frameborder=""0"" allow=""autoplay; encrypted - media"" allowfullscreen></iframe>
        </div>
    </body>
</html>";
            webBrowser1.DocumentText = page;
        }
        
        private void btnNew_Click(object sender, System.EventArgs e)
            => Editor.Instance.CreateNewProject();
        private void btnOpen_Click(object sender, System.EventArgs e)
            => Editor.Instance.OpenProject();

        public void AddRecentFilePath(string path)
        {
            LinkLabel link = new LinkLabel()
            {
                Text = path,
                Font = new Font("Segoe UI", 10.0f, FontStyle.Regular),
                ForeColor = Color.FromArgb(224, 224, 224),
                AutoSize = true,
                Margin = new Padding(5),
            };
            link.LinkClicked += Link_LinkClicked;
            flowLayoutPanel1.Controls.Add(link);
        }

        private void Link_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            
        }
    }
}
