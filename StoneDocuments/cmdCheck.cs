#region Namespaces
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

#endregion

namespace StoneDocuments
{
    [Transaction(TransactionMode.Manual)]
    public class CmdCheck : IExternalCommand
    {
        public Result Execute(
          ExternalCommandData commandData,
          ref string message,
          ElementSet elements)
        {
            UIApplication uiapp = commandData.Application;
            UIDocument uidoc = uiapp.ActiveUIDocument;
            Application app = uiapp.Application;
            Document doc = uidoc.Document;

            // check current view - make sure it's a sheet
            ViewSheet curSheet;
            if (doc.ActiveView is ViewSheet)
            {
                curSheet = doc.ActiveView as ViewSheet;
            }
            else
            {
                TaskDialog.Show("Error", "Please make the active view a sheet");
                return Result.Failed;
            }

            // get schedule from sheet
            List<ViewSchedule> schedList = Utils.GetAllSchedulesOnSheet(doc, curSheet);

            // check if sheet has schedule
            if (schedList.Count == 0)
            {
                TaskDialog.Show("Error", "The current sheet does not have a schedule. Please select another sheet.");
                return Result.Failed;
            }

            // get elements from schedule
            List<Element> elemList = Utils.GetElementsFromSchedule(doc, schedList[0]);
            List<ElementId> elemIdList = Utils.GetElementIdsFromList(doc, elemList);

            // set current view to 3D view
            View curView;
            curView = Utils.GetViewByName(doc, "{3D}");

            uidoc.ActiveView = curView;

            uidoc.Selection.SetElementIds(elemIdList);

            // create handler and event then open form
            RequestHandler handler = new RequestHandler();
            ExternalEvent exEvent = ExternalEvent.Create(handler);
            checkForm curForm = new checkForm(exEvent, handler, elemList.Count);
            curForm.TopMost = true;
            curForm.Show();

            return Result.Succeeded;
        }
    }
    public class RequestHandler : IExternalEventHandler
    {
        public String GetName()
        {
            return "Change selected element overrides";
        }
        public void Execute(UIApplication uiapp)
        {
            UIDocument uidoc = uiapp.ActiveUIDocument;
            Document doc = uidoc.Document;

            // set override settings
            Autodesk.Revit.DB.Color color = new Autodesk.Revit.DB.Color(255, 0, 0);
            OverrideGraphicSettings colSet = new OverrideGraphicSettings();
            colSet.SetSurfaceForegroundPatternColor(color);

            FillPatternElement curFPE = Utils.GetFillPatternByName(doc, "<Solid fill>");
            colSet.SetSurfaceForegroundPatternId(curFPE.Id);

            ICollection<ElementId> selElements = uidoc.Selection.GetElementIds();

            // update element overrides in view
            using (Transaction t = new Transaction(doc))
            {
                t.Start("Set element color");

                foreach (ElementId curId in selElements)
                {
                    doc.ActiveView.SetElementOverrides(curId, colSet);
                }

                t.Commit();
            }

            selElements.Clear();

            uidoc.Selection.SetElementIds(selElements);

            uidoc.RefreshActiveView();

            return;
        }
    }

    public class CancelHandler : IExternalEventHandler
    {
       UIDocument uidoc = uiapp.ActiveUIDocument;
       Document doc = uidoc.Document;

       ICollection<ElementId> selElements = uidoc.Selection.GetElementIds();

       selElements.Clear();

       uidoc.Selection.SetElementIds(selElements);

       uidoc.RefreshActiveView();

       return;
    }
}
