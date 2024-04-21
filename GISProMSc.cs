using ArcGIS.Core.CIM;
using ArcGIS.Core.Data;
using ArcGIS.Core.Geometry;
using ArcGIS.Desktop.Catalog;
using ArcGIS.Desktop.Core;
using ArcGIS.Desktop.Core.Geoprocessing;
using ArcGIS.Desktop.Editing;
using ArcGIS.Desktop.Extensions;
using ArcGIS.Desktop.Framework;
using ArcGIS.Desktop.Framework.Contracts;
using ArcGIS.Desktop.Framework.Dialogs;
using ArcGIS.Desktop.Framework.Threading.Tasks;
using ArcGIS.Desktop.Internal.Core.Conda;
using ArcGIS.Desktop.KnowledgeGraph;
using ArcGIS.Desktop.Layouts;
using ArcGIS.Desktop.Mapping;
using System;
using System.Collections.Generic;

namespace ArcGISMsc;

internal class MainButton : Button
{
    protected override void OnClick()
    {
        IReadOnlyList<Layer> selectedLayers;
        if (MapView.Active is MapView mapView &&
            (selectedLayers = mapView.GetSelectedLayers()).Count is 1 &&
            selectedLayers[0] is FeatureLayer featureLayer &&
            featureLayer.ShapeType is esriGeometryType.esriGeometryPolyline or esriGeometryType.esriGeometryMultipoint
            or esriGeometryType.esriGeometryPolygon or esriGeometryType.esriGeometryEnvelope)
        {
            QueuedTask.Run(async () =>
            {
                using FeatureClass featureClass = featureLayer.GetFeatureClass();
                long featureCount = featureClass.GetCount();
                if (featureCount is not 0)
                {
                    if (featureCount is 1)
                    {
                        using RowCursor cursor = featureClass.Search(null, false);
                        if (cursor.MoveNext())
                        {
                            using Feature feature = (Feature)cursor.Current;
                            if (feature is not null)
                            {
                                Geometry shape = feature.GetShape();
                                Envelope extent = shape.Extent;

                                IReadOnlyList<string> gpValues = Geoprocessing.MakeValueArray("memory\\FishNet", null, null, 100d, 100d, null, null, null, "NO_LABELS", extent, "POLYGON");

                                const string gpName = "management.CreateFishnet";
                                const GPExecuteToolFlags executeFlags = GPExecuteToolFlags.GPThread | GPExecuteToolFlags.AddToHistory;

                                IGPResult gpResult = await Geoprocessing.ExecuteToolAsync(gpName, gpValues, null, null, null, executeFlags);


                                gpValues = Geoprocessing.MakeValueArray("memory\\FishNet", "FishNetLayer");

                                gpResult = await Geoprocessing.ExecuteToolAsync("management.MakeFeatureLayer", gpValues, null, null, null, executeFlags | GPExecuteToolFlags.AddOutputsToMap);


                                gpValues = Geoprocessing.MakeValueArray("FishNetLayer", "INTERSECT", featureLayer);

                                gpResult = await Geoprocessing.ExecuteToolAsync("management.SelectLayerByLocation", gpValues, null, null, null, executeFlags);


                                gpValues = Geoprocessing.MakeValueArray("FishNetLayer", "memory\\FishNetOp");

                                gpResult = await Geoprocessing.ExecuteToolAsync("management.CopyFeatures", gpValues, null, null, null, GPExecuteToolFlags.AddOutputsToMap | GPExecuteToolFlags.GPThread | GPExecuteToolFlags.AddToHistory | GPExecuteToolFlags.RefreshProjectItems);

                            }
                        }
                    }
                }
            });
        }
    }
}
