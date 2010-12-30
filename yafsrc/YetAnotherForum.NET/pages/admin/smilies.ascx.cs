/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bj�rnar Henden
 * Copyright (C) 2006-2010 Jaben Cargman
 * http://www.yetanotherforum.net/
 * 
 * This program is free software; you can redistribute it and/or
 * modify it under the terms of the GNU General Public License
 * as published by the Free Software Foundation; either version 2
 * of the License, or (at your option) any later version.
 * 
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 * 
 * You should have received a copy of the GNU General Public License
 * along with this program; if not, write to the Free Software
 * Foundation, Inc., 59 Temple Place - Suite 330, Boston, MA  02111-1307, USA.
 */

namespace YAF.Pages.Admin
{
  #region Using

  using System;
  using System.Data;
  using System.Web.UI.WebControls;

  using YAF.Classes.Data;
  using YAF.Core;
  using YAF.Core.BBCode;
  using YAF.Core.Services;
  using YAF.Types;
  using YAF.Types.Constants;
  using YAF.Utils;

  #endregion

  /// <summary>
  /// Summary description for smilies.
  /// </summary>
  public partial class smilies : AdminPage
  {
    #region Methods

    /// <summary>
    /// The delete_ load.
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The e.
    /// </param>
    protected void Delete_Load([NotNull] object sender, [NotNull] EventArgs e)
    {
      ((LinkButton)sender).Attributes["onclick"] = "return confirm('Delete this smiley?')";
    }

    /// <summary>
    /// The on init.
    /// </summary>
    /// <param name="e">
    /// The e.
    /// </param>
    protected override void OnInit([NotNull] EventArgs e)
    {
      this.Pager.PageChange += this.Pager_PageChange;
      this.List.ItemCommand += this.List_ItemCommand;

      // CODEGEN: This call is required by the ASP.NET Web Form Designer.
      InitializeComponent();
      base.OnInit(e);
    }

    /// <summary>
    /// The page_ load.
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The e.
    /// </param>
    protected void Page_Load([NotNull] object sender, [NotNull] EventArgs e)
    {
      if (!this.IsPostBack)
      {
        this.PageLinks.AddLink(this.PageContext.BoardSettings.Name, YafBuildLink.GetLink(ForumPages.forum));
        this.PageLinks.AddLink(this.GetText("ADMIN_ADMIN", "Administration"), string.Empty);
        this.PageLinks.AddLink("Smilies", string.Empty);

        this.BindData();
      }
    }

    /// <summary>
    /// The bind data.
    /// </summary>
    private void BindData()
    {
      this.Pager.PageSize = 25;
      DataView dv = DB.smiley_list(this.PageContext.PageBoardID, null).DefaultView;
      this.Pager.Count = dv.Count;
      var pds = new PagedDataSource();
      pds.DataSource = dv;
      pds.AllowPaging = true;
      pds.CurrentPageIndex = this.Pager.CurrentPageIndex;
      pds.PageSize = this.Pager.PageSize;
      this.List.DataSource = pds;
      this.DataBind();
    }

    /// <summary>
    /// Required method for Designer support - do not modify
    ///   the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent()
    {
    }

    /// <summary>
    /// The list_ item command.
    /// </summary>
    /// <param name="source">
    /// The source.
    /// </param>
    /// <param name="e">
    /// The e.
    /// </param>
    private void List_ItemCommand([NotNull] object source, [NotNull] RepeaterCommandEventArgs e)
    {
      switch (e.CommandName)
      {
        case "add":
          YafBuildLink.Redirect(ForumPages.admin_smilies_edit);
          break;
        case "edit":
          YafBuildLink.Redirect(ForumPages.admin_smilies_edit, "s={0}", e.CommandArgument);
          break;
        case "moveup":
          DB.smiley_resort(this.PageContext.PageBoardID, e.CommandArgument, -1);

          // invalidate the cache...
          this.PageContext.Cache.Remove(YafCache.GetBoardCacheKey(Constants.Cache.Smilies));
          this.BindData();
          ReplaceRulesCreator.ClearCache();
          break;
        case "movedown":
          DB.smiley_resort(this.PageContext.PageBoardID, e.CommandArgument, 1);

          // invalidate the cache...
          this.PageContext.Cache.Remove(YafCache.GetBoardCacheKey(Constants.Cache.Smilies));
          this.BindData();
          ReplaceRulesCreator.ClearCache();
          break;
        case "delete":
          DB.smiley_delete(e.CommandArgument);

          // invalidate the cache...
          this.PageContext.Cache.Remove(YafCache.GetBoardCacheKey(Constants.Cache.Smilies));
          this.BindData();
          ReplaceRulesCreator.ClearCache();
          break;
        case "import":
          YafBuildLink.Redirect(ForumPages.admin_smilies_import);
          break;
      }
    }

    /// <summary>
    /// The pager_ page change.
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The e.
    /// </param>
    private void Pager_PageChange([NotNull] object sender, [NotNull] EventArgs e)
    {
      this.BindData();
    }

    #endregion
  }
}