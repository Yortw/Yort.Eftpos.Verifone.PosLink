using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Yort.Eftpos.Verifone.PosLink
{
	internal partial class PosLinkDialog : Form
	{
		internal PosLinkDialog()
		{
			InitializeComponent();
		}

		internal void Center()
		{
			this.CenterToParent();
		}
	}
}