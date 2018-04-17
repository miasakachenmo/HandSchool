﻿using HandSchool.Internal;
using HandSchool.Services;
using System.Text;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace HandSchool.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class WebViewPage : PopContentPage
	{
        private IInfoEntrance InfoEntrance { get; }

		public WebViewPage(IInfoEntrance entrance)
		{
			InitializeComponent();
            Title = entrance.Name;
            InfoEntrance = entrance;
            InfoEntrance.Binding = new ViewResponse(this);
            var sb = new StringBuilder();
            InfoEntrance.HtmlDocument.ToHtml(sb);
            WebView.Html = sb.ToString();
            foreach (var key in InfoEntrance.Menu)
                ToolbarItems.Add(new ToolbarItem { Text = key.Name, Command = key.Command });
            entrance.Evaluate = WebView.JavaScript;
        }
    }
}