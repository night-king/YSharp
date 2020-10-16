using Microsoft.Extensions.DependencyInjection;
using YSharp.Domain;
using YSharp.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Text;
using System.Xml;

namespace YSharp.Admin.Helpers
{
    public class MenuHelper
    {
        public string CurrentUrl { private set; get; }

        public IPrincipal Principal { private set; get; }

        private List<MenuViewModel> _userMenus;
        public List<MenuViewModel> UserMenus
        {
            get
            {
                if (_userMenus == null)
                {
                    _userMenus = initUserMenu();
                }
                return _userMenus;
            }
        }
        private List<MenuViewModel> _allMenus;
        public List<MenuViewModel> AllMenus
        {
            get
            {
                if (_allMenus == null)
                {
                    _allMenus = initAllMenu();
                }
                return _allMenus;
            }
        }
        public List<MenuViewModel> AllToList()
        {
            var model = new List<MenuViewModel>();
            if (AllMenus != null)
            {
                foreach (var menu in AllMenus)
                {
                    model.Add(menu);
                    appendAllToList(menu, model);
                }
            }
            return model;
        }
        private void appendAllToList(MenuViewModel current, List<MenuViewModel> model)
        {
            if (current.Items != null)
            {
                foreach (var menu in current.Items)
                {
                    model.Add(menu);
                    appendAllToList(menu, model);
                }
            }
        }
        public MenuHelper(string currentUrl, IPrincipal principal)
        {
            CurrentUrl = currentUrl;
            Principal = principal;
        }
        public MenuToolBarModel GetToolBar()
        {
            if (UserMenus == null || UserMenus.Count == 0)
            {
                return new MenuToolBarModel();
            }
            return new MenuToolBarModel
            {
                Current = getCurrent(),
                Buttons = getToolBarButtons(),
            };
        }
        public IEnumerable<MenuViewModel> GetAllMenu()
        {
            return AllMenus;
        }

        public MenuLeftModel GetLeftMenu()
        {
            return new MenuLeftModel { Items = UserMenus };
        }

        public MenuTableMenuModel GetTableMenu(IEnumerable<string> excludes)
        {
            var current = getCurrent();
            if (current == null || current.Items == null || current.Items.Count == 0)
            {
                return new MenuTableMenuModel { };
            }
            var query = current.Items.Where(x => x.IsDisplayOnTable == true);
            if (excludes != null && excludes.Where(x => !string.IsNullOrEmpty(x)).Count() > 0)
            {
                var excludeLowers = excludes.Where(x => !string.IsNullOrEmpty(x)).Select(x => x.ToLower());
                query = query.Where(x => !excludeLowers.Contains(x.Action.ToLower()));
            }
            return new MenuTableMenuModel { Items = query.ToList() };
        }
        private List<MenuViewModel> initAllMenu()
        {
            var model = new List<MenuViewModel>();
            var xmlPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "menu.xml");
            XmlDocument doc = new XmlDocument();
            doc.Load(xmlPath);
            var root = doc.SelectSingleNode("menus");
            foreach (XmlNode xn in root.ChildNodes)
            {
                var menu = readMenu(xn);
                model.Add(menu);
                appendChildren(model, xn, menu, CurrentUrl, true, null);
            }
            return model;
        }
        private List<MenuViewModel> initUserMenu()
        {
            var model = new List<MenuViewModel>();
            var claimsIdentity = Principal.Identity as ClaimsIdentity;
            var nameIdentifier = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            var userId = nameIdentifier == null ? "" : nameIdentifier.Value;
            if (string.IsNullOrEmpty(userId)) { return model; }

            //step1：获取登陆用户Permissions
            var isSuperAdmin = false;
            var permissions = new List<string>();
            using (var serviceScope = ApplicationBuilder.Instance.ApplicationServices.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                var uow = serviceScope.ServiceProvider.GetService<IUnitOfWork>();
                var user = uow.Set<ApplicationUser>().Find(userId);
                if (user == null) { return model; }
                if (user.IsSuperAdmin)
                {
                    isSuperAdmin = true;
                }
                else
                {
                    isSuperAdmin = false;
                    var userRoles = claimsIdentity.FindAll(ClaimTypes.Role).Select(x => x.Value).ToList();
                    permissions = uow.Set<RolePermission>().Where(x => userRoles.Contains(x.Role.Name)).ToList().Select(x => x.PermissionId).ToList();
                }
            }
            //step2: 读取xml,组装Model
            var xmlPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "menu.xml");
            XmlDocument doc = new XmlDocument();
            doc.Load(xmlPath);
            var root = doc.SelectSingleNode("menus");
            foreach (XmlNode xn in root.ChildNodes)
            {
                var menu = readMenu(xn);
                if (isSuperAdmin == false && permissions.Any(x => x == menu.Id) == false)
                {
                    continue;
                }
                var isActive = menu.Action == null ? false : menu.Action.ToLower() == CurrentUrl.ToLower() || (CurrentUrl.ToLower() != "/" && CurrentUrl.ToLower() != "/home/index" && CurrentUrl.ToLower() != "/admin" && menu.RelevantURL != null && menu.RelevantURL.ToLower().Contains(CurrentUrl.ToLower()));
                if (isActive)
                {
                    setActive(menu);
                }
                model.Add(menu);
                appendChildren(model, xn, menu, CurrentUrl, isSuperAdmin, permissions);
            }
            return model;
        }
        private void appendChildren(List<MenuViewModel> model, XmlNode node, MenuViewModel parent, string url, bool isSuperAdmin, IEnumerable<string> permissions)
        {
            foreach (XmlNode xn in node.ChildNodes)
            {
                var menu = readMenu(xn);
                if (isSuperAdmin == false && permissions.Any(x => x == menu.Id) == false)
                {
                    continue;
                }
                menu.Parent = parent;
                var isActive = menu.Action == null ? false : menu.Action.ToLower() == url.ToLower() || (url.ToLower() != "/" && url.ToLower() != "/home/index" && url.ToLower() != "/admin" && menu.RelevantURL != null && menu.RelevantURL.ToLower().Contains(url.ToLower()));
                if (isActive)
                {
                    setActive(menu);
                }
                if (parent.Items == null)
                {
                    parent.Items = new List<MenuViewModel>();
                }
                parent.Items.Add(menu);
                appendChildren(model, xn, menu, url, isSuperAdmin, permissions);
            }
        }
        private void setActive(MenuViewModel menu)
        {
            menu.IsActive = true;
            if (menu.Parent != null)
            {
                menu.Parent.IsActive = true;
                setActive(menu.Parent);
            }
        }

        private MenuViewModel readMenu(XmlNode xn1)
        {
            var xe1 = (XmlElement)xn1;
            var id = xe1.GetAttribute("id");
            var name = xe1.GetAttribute("name");
            var title = xe1.GetAttribute("title");
            var icon = xe1.GetAttribute("icon");
            var left = xe1.HasAttribute("left") && xe1.GetAttribute("left") == "true";
            var url = xe1.GetAttribute("url");
            var exUrls = xe1.GetAttribute("exUrls");
            var style = xe1.GetAttribute("style");
            var width = xe1.HasAttribute("width") ? int.Parse(xe1.GetAttribute("width")) : 0;
            var height = xe1.HasAttribute("height") ? int.Parse(xe1.GetAttribute("height")) : 0;
            var nid = xe1.HasAttribute("nid") && xe1.GetAttribute("nid") == "true";
            var table = xe1.HasAttribute("table") && xe1.GetAttribute("table") == "true";
            var opennew = xe1.HasAttribute("opennew") && xe1.GetAttribute("opennew") == "true";
            var toolbar = xe1.HasAttribute("toolbar") && xe1.GetAttribute("toolbar") == "true";
            return new MenuViewModel
            {
                Id = id,
                RelevantURL = exUrls,
                Action = url,
                Text = name,
                Title = string.IsNullOrEmpty(title) ? name : title,
                Style = style,
                Width = width,
                Height = height,
                Icon = icon,
                IsDisplayOnMenu = left,
                IsMustSelected = nid,
                IsOpenNew = opennew,
                IsDisplayOnTable = table,
                IsDisplayOnToolbar = toolbar,
            };
        }
        private List<MenuViewModel> getToolBarButtons()
        {
            var buttons = new List<MenuViewModel>();
            var actived = UserMenus.FirstOrDefault(x => x.IsActive == true);
            findButtons(actived, buttons, CurrentUrl);
            return buttons;

        }
        private void findButtons(MenuViewModel actived, List<MenuViewModel> buttons, string url)
        {
            if (actived == null || actived.Items == null)
            {
                return;
            }
            foreach (var bt in actived.Items)
            {
                if (bt.IsDisplayOnToolbar && actived.Action.ToLower() == url.ToLower())
                {
                    buttons.Add(bt);
                }
                findButtons(bt, buttons, url);
            }
        }

        private MenuViewModel getCurrent()
        {
            if (UserMenus != null)
            {
                var active = UserMenus.FirstOrDefault(x => x.Parent == null && x.IsActive);
                if (active != null)
                {
                    return findCurrent(active);
                }
                else
                {
                    return active;
                }
            }
            return null;
        }
        private MenuViewModel findCurrent(MenuViewModel menu)
        {
            if (menu.Action.ToLower() == CurrentUrl.ToLower())
            {
                return menu;
            }
            else
            {
                if (menu.Items == null)
                {
                    return null;
                }
                var active = menu.Items.FirstOrDefault(x => x.IsActive);
                if (active == null)
                {
                    return null;
                }
                return findCurrent(active);
            }
        }

    }
    public class MenuToolBarModel
    {
        public MenuViewModel Current { set; get; }

        public List<MenuViewModel> Buttons { set; get; }

    }
    public class MenuLeftModel
    {
        public List<MenuViewModel> Items { set; get; }

    }
    public class MenuTableMenuModel
    {
        public List<MenuViewModel> Items { set; get; }

    }

    public class MenuViewModel
    {
        public string Id { set; get; }
        public string Text { get; set; }
        public string Title { get; set; }

        public string Action { get; set; }
        public string Icon { get; set; }
        public string RelevantURL { get; set; }
        public string Style { get; set; }

        public int Width
        {
            get; set;
        }

        public int Height
        {
            get; set;
        }

        public bool IsMustSelected { get; set; }
        public bool IsDisplayOnTable { get; set; }
        public bool IsOpenNew { get; set; }
        public bool IsDisplayOnMenu { get; set; }

        public bool IsDisplayOnToolbar { get; set; }
        public bool IsActive { get; set; }
        public MenuViewModel Parent { get; set; }

        public ICollection<MenuViewModel> Items { get; set; }

    }
}
