﻿using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Settings;
using Implem.Pleasanter.Models;
using Implem.PleasanterFilters;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
namespace Implem.Pleasanter.Controllers
{
    [Authorize]
    public class TenantsController : Controller
    {
        [AcceptVerbs(HttpVerbs.Get, HttpVerbs.Post)]
        public ActionResult Edit()
        {
            var context = new Context();
            if (!context.Ajax)
            {
                var log = new SysLogModel(context: context);
                var html = TenantUtilities.Editor(
                    context: context,
                    ss: SiteSettingsUtilities.TenantsSiteSettings(context: context),
                    tenantId: context.TenantId,
                    clearSessions: true);
                ViewBag.HtmlBody = html;
                log.Finish(context: context, responseSize: html.Length);
                return View();
            }
            else
            {
                var log = new SysLogModel(context: context);
                var json = TenantUtilities.EditorJson(
                    context: context,
                    ss: SiteSettingsUtilities.TenantsSiteSettings(context: context),
                    tenantId: context.TenantId);
                log.Finish(context: context, responseSize: json.Length);
                return Content(json);
            }
        }

        [HttpPut]
        public string Update()
        {
            var context = new Context();
            var log = new SysLogModel(context: context);
            var json = TenantUtilities.Update(
                context: context,
                ss: SiteSettingsUtilities.TenantsSiteSettings(context: context),
                tenantId: context.TenantId);
            log.Finish(context: context, responseSize: json.Length);
            return json;
        }
    }
}
