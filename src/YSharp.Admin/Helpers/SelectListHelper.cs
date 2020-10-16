using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace YSharp.Admin.Helpers
{
    public class SelectListHelper
    {
        public static SelectList Create(Dictionary<string, string> input, string selected = "", bool isAddDefault = false, string defaultText = "ALL")
        {
            var items = new List<SelectListItem>();
            if (isAddDefault)
            {
                items.Add(new SelectListItem
                {
                    Selected = false,
                    Text = defaultText,
                    Value = "",
                });
            }
            if (input != null && input.Count > 0)
            {
                foreach (var item in input)
                {
                    items.Add(new SelectListItem
                    {
                        Selected = item.Key == selected,
                        Text = item.Value,
                        Value = item.Key,
                    });
                }
            }

            return new SelectList(items, "Value", "Text", selected);
        }
        public static SelectList Create(Dictionary<string, string> input, IEnumerable<string> selecteds, bool isAddDefault = false, string defaultText = "ALL")
        {
            var items = new List<SelectListItem>();
            if (isAddDefault)
            {
                items.Add(new SelectListItem
                {
                    Selected = false,
                    Text = defaultText,
                    Value = "",
                });
            }
            if (input != null && input.Count > 0)
            {
                foreach (var item in input)
                {
                    items.Add(new SelectListItem
                    {
                        Selected = selecteds.Contains(item.Key),
                        Text = item.Value,
                        Value = item.Key,
                    });
                }
            }
            return new SelectList(items, "Value", "Text");
        }
    }
}
