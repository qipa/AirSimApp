﻿using AirSimApp.Models;
using DotSpatial.Positioning;
using DotSpatialExtensions;
using MapControl;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;

namespace AirSimApp.ViewModels
{
    /// <summary>View model for map.</summary>
    public class MapViewModel : PropertyChangedBase, IDisposable
    {
        /// <summary>Wire up to model.</summary>
        public MapViewModel(MultirotorVehicleModel vehicle)
        {
            _vehicle = vehicle;

            _vehicle.PropertyChanged += onVehiclePropertyChanged;
        }

        /// <summary>Center location of map.</summary>
        public Location Center { get => _center; set => SetProperty(ref _center, value); }

        /// <summary>Vehicle's home location.</summary>
        public Location Home { get => _home; set => SetProperty(ref _home, value); }

        /// <summary>The underlaying map layer.</summary>
        public UIElement Layer { get => _mapLayers[_mapLayerName]; }

        /// <summary>Underlaying map layer.</summary>
        public string LayerName
        {
            get => _mapLayerName; set
            {
                if (_mapLayerName != value && _mapLayers.ContainsKey(value))
                {
                    _mapLayerName = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(Layer));
                }
            }
        }

        /// <summary>All map layer names.</summary>
        public IEnumerable<string> LayerNames => _mapLayers.Keys;

        /// <summary>Max allowed zoom level.</summary>
        public int MaxZoomLevel { get => _maxZoomLevel; set => SetProperty(ref _maxZoomLevel, value); }

        /// <summary>Projection type to use.</summary>
        public MapProjection Projection { get; } = new WebMercatorProjection();

        public double VehicleHeading { get => _vehicleHeading; set => SetProperty(ref _vehicleHeading, value); }

        /// <summary>Vehicle location.</summary>
        public Location VehicleLocation { get => _vehicleLocation; set => SetProperty(ref _vehicleLocation, value); }

        /// <summary>Current zoom level.</summary>
        public int ZoomLevel { get => _zoomLevel; set => SetProperty(ref _zoomLevel, value); }

        /// <inheritdoc cref="IDisposable.Dispose" />
        public void Dispose()
        {
            if (!_disposed)
            {
                _vehicle.PropertyChanged -= onVehiclePropertyChanged;
            }
        }

        private readonly Dictionary<string, UIElement> _mapLayers = new Dictionary<string, UIElement>
        {
            {
                "OpenStreetMap",
                MapTileLayer.OpenStreetMapTileLayer
            },
            {
                "OpenStreetMap German",
                new MapTileLayer
                {
                    SourceName = "OpenStreetMap German",
                    Description = "© [OpenStreetMap contributors](http://www.openstreetmap.org/copyright)",
                    TileSource = new TileSource { UriFormat = "http://{c}.tile.openstreetmap.de/tiles/osmde/{z}/{x}/{y}.png" },
                    MaxZoomLevel = 19
                }
            },
            {
                "Stamen Terrain",
                new MapTileLayer
                {
                    SourceName = "Stamen Terrain",
                    Description = "Map tiles by [Stamen Design](http://stamen.com/), under [CC BY 3.0](http://creativecommons.org/licenses/by/3.0)\nData by [OpenStreetMap](http://openstreetmap.org/), under [ODbL](http://www.openstreetmap.org/copyright)",
                    TileSource = new TileSource { UriFormat = "http://tile.stamen.com/terrain/{z}/{x}/{y}.png" },
                    MaxZoomLevel = 17
                }
            },
            {
                "Stamen Toner Light",
                new MapTileLayer
                {
                    SourceName = "Stamen Toner Light",
                    Description = "Map tiles by [Stamen Design](http://stamen.com/), under [CC BY 3.0](http://creativecommons.org/licenses/by/3.0)\nData by [OpenStreetMap](http://openstreetmap.org/), under [ODbL](http://www.openstreetmap.org/copyright)",
                    TileSource = new TileSource { UriFormat = "http://tile.stamen.com/toner-lite/{z}/{x}/{y}.png" },
                    MaxZoomLevel = 18
                }
            },
            {
                "Seamarks",
                new MapTileLayer
                {
                    SourceName = "OpenSeaMap",
                    TileSource = new TileSource { UriFormat = "http://tiles.openseamap.org/seamark/{z}/{x}/{y}.png" },
                    MinZoomLevel = 9,
                    MaxZoomLevel = 18
                }
            },
            {
                "OpenStreetMap WMS",
                new WmsImageLayer
                {
                    Description = "© [terrestris GmbH & Co. KG](http://ows.terrestris.de/)\nData © [OpenStreetMap contributors](http://www.openstreetmap.org/copyright)",
                    ServerUri = new Uri("http://ows.terrestris.de/osm/service"),
                    Layers = "OSM-WMS"
                }
            },
            {
                "OpenStreetMap TOPO WMS",
                new WmsImageLayer
                {
                    Description = "© [terrestris GmbH & Co. KG](http://ows.terrestris.de/)\nData © [OpenStreetMap contributors](http://www.openstreetmap.org/copyright)",
                    ServerUri = new Uri("http://ows.terrestris.de/osm/service"),
                    Layers = "TOPO-OSM-WMS"
                }
            },
            {
                "SevenCs ChartServer",
                new WmsImageLayer
                {
                    Description = "© [SevenCs GmbH](http://www.sevencs.com)",
                    ServerUri = new Uri("http://chartserver4.sevencs.com:8080"),
                    Layers = "ENC",
                    MaxBoundingBoxWidth = 360
                }
            }
        };

        private readonly MultirotorVehicleModel _vehicle;
        private Location _center = new Location(0, 0);
        private bool _disposed = false;
        private Location _home = new Location(0, 0);
        private string _mapLayerName = "OpenStreetMap";
        private int _maxZoomLevel = 20;
        private double _vehicleHeading = 0;
        private Location _vehicleLocation = new Location(0, 0);
        private int _zoomLevel = 1;

        private void onVehiclePropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case (nameof(_vehicle.HomeLocation)):
                    Home = new Location(_vehicle.HomeLocation.Latitude.InDegrees(), _vehicle.HomeLocation.Longitude.InDegrees());
                    break;

                case (nameof(_vehicle.VehicleLocation)):
                    Center = new Location(_vehicle.VehicleLocation.Latitude.InDegrees(), _vehicle.VehicleLocation.Longitude.InDegrees());
                    VehicleLocation = new Location(_vehicle.VehicleLocation.Latitude.InDegrees(), _vehicle.VehicleLocation.Longitude.InDegrees());
                    break;

                case (nameof(_vehicle.VehicleYaw)):
                    VehicleHeading = (_vehicle.VehicleYaw.InDegrees() - 90.0);
                    break;
            }
        }
    }
}