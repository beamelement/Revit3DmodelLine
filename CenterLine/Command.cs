using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;

namespace CenterLine
{
    [Transaction(TransactionMode.Manual)]
    public class Command : IExternalCommand
    {
        Curve FlatModelLine;
        Curve VerticalModelLine;
        SketchPlane VerticalModelLineSketchPlane;

        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            Document revitDoc = commandData.Application.ActiveUIDocument.Document;  //取得文档           
            Autodesk.Revit.ApplicationServices.Application revitApp = commandData.Application.Application;             //取得应用程序            
            UIDocument uiDoc = commandData.Application.ActiveUIDocument;           //取得当前活动文档        


            //新建一个窗口
            Window1 window1 = new Window1();

            if (window1.ShowDialog() == true)
            {
                //窗口打开并停留，只有点击按键之后，窗口关闭并返回true
            }

            while (!window1.Done)
            {
                //选择平曲线
                if (window1.FlatCurve)
                {

                    //因为要对原有模型线进行一个删除是对文件进行一个删除，故要创建一个事件
                    using(Transaction transaction = new Transaction(uiDoc.Document))
                    {
                        transaction.Start("删除平曲线");

                        Selection sel = uiDoc.Selection;
                        Reference ref1 = sel.PickObject(ObjectType.Element, "选择一条模型线作为平曲线");
                        Element elem = revitDoc.GetElement(ref1);
                        ModelLine modelLine = elem as ModelLine;
                        //做一个判断，判断其是否为ModelNurbSpline
                        if (modelLine == null)
                        {
                            ModelNurbSpline modelNurbSpline = elem as ModelNurbSpline;
                            FlatModelLine = modelNurbSpline.GeometryCurve;
                        }
                        else
                        {
                            FlatModelLine = modelLine.GeometryCurve;
                        }

                        
                        //1、清除平曲线  2、重置window1.FlatCurve
                        uiDoc.Document.Delete(elem.Id);
                        window1.FlatCurve = false;
                        //window1.Visibility = System.Windows.Visibility.Visible;

                        transaction.Commit();
                    }

                }

                if (window1.ShowDialog() == true)
                {
                    //窗口打开并停留，只有点击按键之后，窗口关闭并返回true
                }


                //选择纵曲线
                if (window1.VerticalCurve)
                {
                    //因为要对原有模型线进行一个删除是对文件进行一个删除，故要创建一个事件
                    using (Transaction transaction = new Transaction(uiDoc.Document))
                    {
                        transaction.Start("删除纵曲线");


                        Selection sel = uiDoc.Selection;
                        Reference ref1 = sel.PickObject(ObjectType.Element, "选择一条模型线作为纵曲线");
                        Element elem = revitDoc.GetElement(ref1);
                        ModelLine modelLine = elem as ModelLine;

                        //做一个判断，判断其是否为ModelNurbSpline
                        if (modelLine == null)
                        {
                            ModelNurbSpline modelNurbSpline = elem as ModelNurbSpline;
                            VerticalModelLine= modelNurbSpline.GeometryCurve;
                        }
                        else
                        {
                            VerticalModelLine = modelLine.GeometryCurve;
                        }




                        //VerticalModelLineSketchPlane = modelLine.SketchPlane;

                        //1、清除纵曲线   2、重置window1.VerticalCurve
                        uiDoc.Document.Delete(elem.Id);
                        window1.VerticalCurve = false;
                        //window1.Visibility = System.Windows.Visibility.Visible;

                        transaction.Commit();
                    }
                }

                if (window1.ShowDialog() == true)
                {
                    //窗口打开并停留，只有点击按键之后，窗口关闭并返回true
                }
            }



            //获取平曲线上100个点
            //计算首尾两点X轴差值以及曲线长度
            double FdelX=  FlatModelLine.Evaluate(0, true).X - FlatModelLine.Evaluate(1, true).X;
            double FLength = FlatModelLine.Length;
            double FY;

            //选谁的Y最小就选谁
            if(FlatModelLine.Evaluate(0, true).Y <= FlatModelLine.Evaluate(1, true).Y)
            {
                FY = FlatModelLine.Evaluate(0, true).Y;
            }
            else
            {
                FY = FlatModelLine.Evaluate(1, true).Y;
            }



            List<XYZ> FlatCurvePointList = new List<XYZ>();
            for (int i = 0; i < 100; i +=1)
            {
                //设置起点和终点，并创建直线
                XYZ point1 = new XYZ(FlatModelLine.Evaluate(0, true).X - i*FdelX/100, FY, FlatModelLine.Evaluate(0, true).Z);
                XYZ point2 = new XYZ(FlatModelLine.Evaluate(0, true).X - i * FdelX / 100, FY+ FLength, FlatModelLine.Evaluate(0, true).Z); 
                Line line = Line.CreateBound(point1, point2);
                Curve curve = line as Curve;

                //求交点并把交点放到集里面去
                IntersectionResultArray intersectionResultArray;
                FlatModelLine.Intersect(curve, out intersectionResultArray);
                XYZ xYZ = intersectionResultArray.get_Item(0).XYZPoint;

                FlatCurvePointList.Add(xYZ);
            }


            //获取纵曲线上100个点
            //计算首尾两点X轴差值以及曲线长度
            double VdelX = VerticalModelLine.Evaluate(0, true).X - VerticalModelLine.Evaluate(1, true).X;
            double VLength = VerticalModelLine.Length;
            double VY;


            //选谁的Y最小就选谁
            if (VerticalModelLine.Evaluate(0, true).Y <= VerticalModelLine.Evaluate(1, true).Y)
            {
                VY = VerticalModelLine.Evaluate(0, true).Y;
            }
            else
            {
                VY = VerticalModelLine.Evaluate(1, true).Y;
            }



            List<XYZ> VerticalCurvePointList = new List<XYZ>()   ;
            for (int i = 0; i < 100; i += 1)
            {
                //设置起点和终点，并创建直线
                XYZ point1 = new XYZ(VerticalModelLine.Evaluate(0, true).X - i * VdelX / 100, VY, VerticalModelLine.Evaluate(0, true).Z);
                XYZ point2 = new XYZ(VerticalModelLine.Evaluate(0, true).X - i * VdelX / 100, VY + VLength, VerticalModelLine.Evaluate(0, true).Z);
                Line line = Line.CreateBound(point1, point2);
                Curve curve = line as Curve;

                //求交点并把交点放到集里面去
                IntersectionResultArray intersectionResultArray;
                VerticalModelLine.Intersect(curve, out intersectionResultArray);
                XYZ xYZ = intersectionResultArray.get_Item(0).XYZPoint;
                VerticalCurvePointList.Add(xYZ);
            }

            //把平曲线和总曲线上点进行整合生成三维曲线
            List<XYZ> CurvePoint = new List<XYZ>(); 
            for (int i = 0; i < 100; i += 1)
            {
                XYZ point = new XYZ(FlatCurvePointList[i].X,FlatCurvePointList[i].Y,VerticalCurvePointList[i].Y);
                CurvePoint.Add(point);
            }

            PolyLine.Create(CurvePoint);


            //让这条线以模型线的形式展示一下

            using (Transaction tran = new Transaction(uiDoc.Document))
            {
                tran.Start("创建模型线");

                for(int i = 0; i < 99; i += 1)
                {
                    XYZ PointStart = CurvePoint[i];
                    XYZ PointEnd = CurvePoint[i+1];
                    //直线向量
                    XYZ vector = new XYZ(PointStart.X - PointEnd.X, PointStart.Y - PointEnd.Y, PointStart.Z - PointEnd.Z);
                    //向量和Z向量叉乘,从而获得一个必定与向量垂直的向量,并以此创建一个平面
                    XYZ normal = vector.CrossProduct(XYZ.BasisZ);
                    Plane plane = Plane.CreateByNormalAndOrigin(normal, PointStart);
                    SketchPlane sketchPlane = SketchPlane.Create(uiDoc.Document, plane);

                    uiDoc.Document.Create.NewModelCurve(Line.CreateBound(PointStart, PointEnd), sketchPlane);

                }

                tran.Commit();

            }



            return Result.Succeeded;
        }
    }
}
