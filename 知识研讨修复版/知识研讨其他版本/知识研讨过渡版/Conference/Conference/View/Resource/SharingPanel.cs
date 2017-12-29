using ConferenceCommon.IconHelper;
using ConferenceCommon.LogHelper;
using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Markup;
namespace Conference.View.Resource
{
	public class SharingPanel : Window, IComponentConnector
	{
		internal TextBlock txtName;
		internal System.Windows.Forms.Panel panel;
		internal System.Windows.Controls.Button btnMini;
		internal System.Windows.Controls.Button btnClose;
		internal System.Windows.Controls.Button btnConnect;
		internal System.Windows.Controls.Button btnDisconnect;
		internal System.Windows.Controls.Button btnPause;
		private bool _contentLoaded;
		public SharingPanel()
		{
			try
			{
				this.InitializeComponent();
				base.Loaded += new RoutedEventHandler(this.MainWindow_Loaded);
				this.btnConnect.Click += new RoutedEventHandler(this.btnConnect_Click);
				this.btnDisconnect.Click += new RoutedEventHandler(this.btnDisconnect_Click);
				this.btnPause.Click += new RoutedEventHandler(this.btnPause_Click);
				this.btnMini.Click += new RoutedEventHandler(this.btnMini_Click);
				this.btnClose.Click += new RoutedEventHandler(this.btnClose_Click);
				this.txtName.Text = Constant.SelfName;
			}
			catch (System.Exception ex)
			{
				LogManage.WriteLog(base.GetType(), ex);
			}
			finally
			{
			}
		}
		private void btnClose_Click(object sender, RoutedEventArgs e)
		{
			try
			{
				System.Windows.Application.Current.Shutdown(0);
			}
			catch (System.Exception ex)
			{
				LogManage.WriteLog(base.GetType(), ex);
			}
			finally
			{
			}
		}
		private void btnMini_Click(object sender, RoutedEventArgs e)
		{
			try
			{
				base.WindowState = WindowState.Minimized;
			}
			catch (System.Exception ex)
			{
				LogManage.WriteLog(base.GetType(), ex);
			}
			finally
			{
			}
		}
		private void btnPause_Click(object sender, RoutedEventArgs e)
		{
			try
			{
				SendKeys.SendWait("^+{p}");
			}
			catch (System.Exception ex)
			{
				LogManage.WriteLog(base.GetType(), ex);
			}
			finally
			{
			}
		}
		private void btnDisconnect_Click(object sender, RoutedEventArgs e)
		{
			try
			{
			}
			catch (System.Exception ex)
			{
				LogManage.WriteLog(base.GetType(), ex);
			}
			finally
			{
			}
		}
		private void btnConnect_Click(object sender, RoutedEventArgs e)
		{
			try
			{
				MainWindow.MainPageInstance.ConversationM.ShareDesk();
			}
			catch (System.Exception ex)
			{
				LogManage.WriteLog(base.GetType(), ex);
			}
			finally
			{
			}
		}
		private void MainWindow_Loaded(object sender, RoutedEventArgs e)
		{
			try
			{
				Win32API.MoveWindow(new WindowInteropHelper(this).Handle, (int)((double)Screen.PrimaryScreen.WorkingArea.Width - base.Width), (int)((double)Screen.PrimaryScreen.WorkingArea.Height - base.Height), (int)base.Width, (int)base.Height, true);
			}
			catch (System.Exception ex)
			{
				LogManage.WriteLog(base.GetType(), ex);
			}
			finally
			{
			}
		}
		private void WindowMove(object sender, MouseButtonEventArgs e)
		{
			try
			{
				if (e.LeftButton == MouseButtonState.Pressed)
				{
					base.DragMove();
				}
			}
			catch (System.Exception ex)
			{
				LogManage.WriteLog(base.GetType(), ex);
			}
		}
		[GeneratedCode("PresentationBuildTasks", "4.0.0.0"), System.Diagnostics.DebuggerNonUserCode]
		public void InitializeComponent()
		{
			if (!this._contentLoaded)
			{
				this._contentLoaded = true;
				Uri resourceLocater = new Uri("/Conference;component/view/resource/sharingpanel.xaml", UriKind.Relative);
				System.Windows.Application.LoadComponent(this, resourceLocater);
			}
		}
		[GeneratedCode("PresentationBuildTasks", "4.0.0.0"), EditorBrowsable(EditorBrowsableState.Never), System.Diagnostics.DebuggerNonUserCode]
		void IComponentConnector.Connect(int connectionId, object target)
		{
			switch (connectionId)
			{
			case 1:
				this.txtName = (TextBlock)target;
				break;
			case 2:
				this.panel = (System.Windows.Forms.Panel)target;
				break;
			case 3:
				this.btnMini = (System.Windows.Controls.Button)target;
				break;
			case 4:
				this.btnClose = (System.Windows.Controls.Button)target;
				break;
			case 5:
				this.btnConnect = (System.Windows.Controls.Button)target;
				break;
			case 6:
				this.btnDisconnect = (System.Windows.Controls.Button)target;
				break;
			case 7:
				this.btnPause = (System.Windows.Controls.Button)target;
				break;
			default:
				this._contentLoaded = true;
				break;
			}
		}
	}
}
