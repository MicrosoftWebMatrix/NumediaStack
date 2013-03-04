using System;
using System.ComponentModel.Composition;
using System.Diagnostics;
using System.Windows.Media.Imaging;
using Microsoft.WebMatrix.Extensibility;
using System.Collections.Generic;
using System.Windows;
using System.Net;
using System.Xml;

namespace NumediaStack
{
    /// <summary>
    /// A sample WebMatrix extension.
    /// </summary>
    [Export(typeof(Extension))]
    public class NumediaStack : Extension
    {

        //--------------------------------------------------------------------------
        //
        //	Variables
        //
        //--------------------------------------------------------------------------



        /// <summary>
        /// Stores a reference to the small star image.
        /// </summary>
        private readonly BitmapImage _starImageSmall = new BitmapImage(new Uri("pack://application:,,,/NumediaStack;component/Star_16x16.png", UriKind.Absolute));

        /// <summary>
        /// Stores a reference to the large star image.
        /// </summary>
        private readonly BitmapImage _starImageLarge = new BitmapImage(new Uri("pack://application:,,,/NumediaStack;component/Star_32x32.png", UriKind.Absolute));

        /// <summary>
        /// Stores a reference to the WebMatrix host interface.
        /// </summary>
        private IWebMatrixHost _webMatrixHost;

        /// <summary>
        /// 
        /// </summary>
        private RibbonGroup _ribbonGroup;

        /// <summary>
        /// 
        /// </summary>
        private RibbonContextualTab _contextualTab;

        /// <summary>
        /// determine if the currently opened siute is NumediaStack
        /// </summary>
        private bool isNumediaStack
        {
            get
            {
                return _webMatrixHost != null
                    && _webMatrixHost.WebSite != null
                    && _webMatrixHost.WebSite.ApplicationIdentifier != null
                    && _webMatrixHost.WebSite.ApplicationIdentifier.ToLower().Contains("numedia");
            }
        }



        //--------------------------------------------------------------------------
        //
        //	Constructors
        //
        //--------------------------------------------------------------------------

        /// <summary>
        /// Initializes a new instance of the NumediaStack class.
        /// </summary>
        public NumediaStack()
            : base("NumediaStack")
        {
        }


        //--------------------------------------------------------------------------
        //
        //	Methods
        //
        //--------------------------------------------------------------------------

        /// <summary>
        /// Called to initialize the extension.
        /// </summary>
        /// <param name="host">WebMatrix host interface.</param>
        /// <param name="initData">Extension initialization data.</param>
        protected override void Initialize(IWebMatrixHost host, ExtensionInitData initData)
        {
            _webMatrixHost = host;
            _webMatrixHost.WebSiteChanged += webMatrixHost_WebSiteChanged;
                                  
            // create a new ribbon button group that contains the get version button
            _ribbonGroup = new RibbonGroup(
                   "NumediaStack",
                   new RibbonItem[]
                    {
                        new RibbonButton(
                            "Get Version",
                            new DelegateCommand(HandleGetVersion),
                            null,
                            _starImageSmall,
                            _starImageLarge)
                    });

            // create a new contextual ribbon tab that will only show up for Numedia Stack 
            _contextualTab = new RibbonContextualTab("Numedia Stack Tools", new RibbonItem[] { _ribbonGroup });
        }
        
        /// <summary>
        /// if the current site is NumediaStack, show the contextual ribbon
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void webMatrixHost_WebSiteChanged(object sender, EventArgs e)
        {
            if (this.isNumediaStack)
            {
                if (!this.ContextualTabItems.Contains(_contextualTab))
                {
                    this.ContextualTabItems.Add(_contextualTab);
                }
                _contextualTab.IsVisible = true;
            }
        }
        

        /// <summary>
        /// get the current version of the app 
        /// </summary>
        /// <param name="parameter">Unused.</param>
        private void HandleGetVersion(object parameter)
        {
            var request = HttpWebRequest.Create(_webMatrixHost.DefaultWebSitePath + "/GetProductInformation.ashx");
            var response = request.GetResponse();
            XmlDocument r = new XmlDocument();
            r.Load(response.GetResponseStream());
            var version = r.ChildNodes[2].ChildNodes[1];
            _webMatrixHost.ShowDialog("NumediaStack Version", "Version " + version, DialogSize.Small, MessageBoxButton.OK);
        }
    }
}
