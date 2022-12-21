using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StoneDocuments
{
    internal static class Utils
    {
        internal static List<ElementId> GetElementIdsFromList(Document doc, List<Element> elemList)
        {
            List<ElementId> returnList = new List<ElementId>();

            foreach (Element curElem in elemList)
                returnList.Add(curElem.Id);

            return returnList;
        }
        internal static List<Element> GetElementsFromSchedule(Document doc, ViewSchedule curView)
        {
            IList<Element> elements = new List<Element>();
            FilteredElementCollector finalCollector = new FilteredElementCollector(doc, curView.Id);

            List<BuiltInCategory> builtInCats = new List<BuiltInCategory>();
            builtInCats.Add(BuiltInCategory.OST_Parts);
            builtInCats.Add(BuiltInCategory.OST_GenericModel);

            ElementMulticategoryFilter filter1 = new ElementMulticategoryFilter(builtInCats);
            finalCollector.WherePasses(filter1);

            return finalCollector.ToElements() as List<Element>;

        }
        internal static List<Element> GetElementsFromView(Document doc, View curView)
        {
            // FilteredElementCollector collector = new FilteredElementCollector(doc, curView.Id);
            // collector.OfCategory(BuiltInCategory.OST_Parts);

            // return collector.ToElements() as List<Element>;

            IList<Element> elements = new List<Element>();
            FilteredElementCollector finalCollector = new FilteredElementCollector(doc, curView.Id);

            List<BuiltInCategory> builtInCats = new List<BuiltInCategory>();
            builtInCats.Add(BuiltInCategory.OST_Parts);
            builtInCats.Add(BuiltInCategory.OST_GenericModel);

            ElementMulticategoryFilter filter1 = new ElementMulticategoryFilter(builtInCats);
            finalCollector.WherePasses(filter1);
            elements = finalCollector.ToElements();

            return finalCollector.ToElements() as List<Element>;

        }
        internal static List<ViewSchedule> GetAllSchedulesOnSheet(Document curDoc, ViewSheet curSheet)
        {
            List<ViewSchedule> schedList = new List<ViewSchedule>();

            FilteredElementCollector curCollector = new FilteredElementCollector(curDoc, curSheet.Id);
            curCollector.OfClass(typeof(ScheduleSheetInstance));

            //loop through views and check if schedule - if so then put into schedule list
            foreach (ScheduleSheetInstance curView in curCollector)
            {
                ViewSchedule curSched = curDoc.GetElement(curView.ScheduleId) as ViewSchedule;
                schedList.Add(curSched);
            }

            return schedList;
        }
        internal static List<View> GetAllViews(Document curDoc)
        {
            FilteredElementCollector m_colviews = new FilteredElementCollector(curDoc);
            m_colviews.OfCategory(BuiltInCategory.OST_Views);

            List<View> m_views = new List<View>();
            foreach (View x in m_colviews.ToElements())
            {
                m_views.Add(x);
            }

            return m_views;
        }
        internal static View GetViewByName(Document curDoc, string viewName)
        {
            List<View> viewList = GetAllViews(curDoc);

            //loop through views in the collector
            foreach (View curView in viewList)
            {
                if (curView.Name == viewName && curView.IsTemplate == false)
                {
                    return curView;
                }
            }

            return null;
        }
        internal static FillPatternElement GetFillPatternByName(Document doc, string name)
        {
            FillPatternElement curFPE = null;

            curFPE = FillPatternElement.GetFillPatternElementByName(doc, FillPatternTarget.Drafting, name);

            return curFPE;
        }
    }
}