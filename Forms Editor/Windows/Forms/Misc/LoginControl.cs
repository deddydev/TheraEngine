using Octokit;
using System;
using System.Windows.Forms;

namespace TheraEditor.Windows.Forms
{
    public partial class LoginControl : UserControl
    {
        public LoginControl()
        {
            InitializeComponent();
        }

        private async void button1_Click(object sender, EventArgs e)
        {
            //GitHubClient github = new GitHubClient(new ProductHeaderValue(Github.RepoName));
            //OauthClient client = new OauthClient(github.Connection);
            //OauthLoginRequest loginReq = new OauthLoginRequest("a32008544eb7682a179a");
            //Uri login = client.GetGitHubLoginUrl(loginReq);
            //OauthTokenRequest tokenReq = new OauthTokenRequest("a32008544eb7682a179a", "739faa679025058e34fc81b720641c85d8676bbe", code);
            //OauthToken token = await client.CreateAccessToken(tokenReq);
        }
    }
}
