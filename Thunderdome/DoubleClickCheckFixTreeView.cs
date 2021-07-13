using System;
using System.Windows.Forms;

namespace Thunderdome
{
    /// <summary>
    /// This class fixes bug with double click check.
    /// When double click occurred on parent item, it does not trigger AfterCheck, so that child items are not updated. 
    /// More details and fix here https://social.msdn.microsoft.com/Forums/windows/en-US/9d717ce0-ec6b-4758-a357-6bb55591f956/possible-bug-in-net-treeview-treenode-checked-state-inconsistent?forum=winforms
    /// </summary>
    internal class DoubleClickCheckFixTreeView : TreeView
    {
        protected override void WndProc(ref Message m)
        {
            // Filter WM_LBUTTONDBLCLK when we're showing check boxes
            if (m.Msg == 0x203 && CheckBoxes)
            {
                int x = m.LParam.ToInt32() & 0xffff;
                int y = (m.LParam.ToInt32() >> 16) & 0xffff;
                TreeViewHitTestInfo hitTestInfo = HitTest(x, y);

                if (hitTestInfo.Node != null && hitTestInfo.Location == TreeViewHitTestLocations.StateImage)
                {
                    OnBeforeCheck(new TreeViewCancelEventArgs(hitTestInfo.Node, false, TreeViewAction.ByMouse));
                    hitTestInfo.Node.Checked = !hitTestInfo.Node.Checked;
                    OnAfterCheck(new TreeViewEventArgs(hitTestInfo.Node, TreeViewAction.ByMouse));
                    m.Result = IntPtr.Zero;
                    return;
                }
            }

            base.WndProc(ref m);
        }
    }
}
