using System;
using ArcGIS.Core.Data;
using ArcGIS.Core.Geometry;
using ArcGIS.Desktop.Framework.Contracts;
using ArcGIS.Desktop.Framework.Threading.Tasks;

namespace TexTruder
{
    internal class MainButton : Button
    {
        protected override async void OnClick()
        {
            const string gdbName = @"F:\TexTest.gdb";
            const string fcName = @"Me104";
            const string textPath = @"F:\Me-104.jpg";
            const long linOid = 1;
            const double extrusion = -2000;


            await QueuedTask.Run(() =>
             {
                 IGeometryEngine engine = GeometryEngine.Instance;
                 using Geodatabase gdb = new(
                     connectionPath: new FileGeodatabaseConnectionPath(
                         path: new Uri(gdbName, UriKind.Absolute)));
                 using FeatureClass fc = gdb.OpenDataset<FeatureClass>(fcName);
                 using RowCursor cursor = fc.Search(
                    queryFilter: new QueryFilter() { ObjectIDs = new long[1] { linOid } },
                     useRecyclingCursor: false);
                 if (cursor.MoveNext())
                 {
                     using Feature feature = (Feature)cursor.Current;
                     Polyline polyline = (Polyline)engine.SetMsAsDistance((Polyline)feature.GetShape(), AsRatioOrLength.AsRatio);
                     ReadOnlyPointCollection coords = polyline.Points;
                     int len = coords.Count;
                     Coordinate2D[] textureCoordinates = GC.AllocateUninitializedArray<Coordinate2D>(2 * len);

                     for (int i = 0, j = 0; i < len;)
                     {
                         double distanceAlong = 1 - coords[i++].M;
                         textureCoordinates[j++].SetComponents(distanceAlong, 0);
                         textureCoordinates[j++].SetComponents(distanceAlong, 1);
                     }

                     MultipatchBuilderEx mpBuilder = new(engine.ConstructMultipatchExtrude(polyline, extrusion));
                     Patch patch = mpBuilder.Patches[0];
                     patch.TextureCoords2D = textureCoordinates;
                     patch.Material = new BasicMaterial()
                     { 
                         TextureResource = new TextureResource(new JPEGTexture(
                         buffer: System.IO.File.ReadAllBytes(textPath)))
                     };

                     using FeatureClass MPStore = gdb.OpenDataset<FeatureClass>(@"MPStore");
                     using InsertCursor insertCursor = MPStore.CreateInsertCursor();
                     using RowBuffer rowBuffer = MPStore.CreateRowBuffer();
                     rowBuffer[1] = mpBuilder.ToGeometry();
                     insertCursor.Insert(rowBuffer);
                     insertCursor.Flush();
                    
                 }
             });
        }
    }
}
