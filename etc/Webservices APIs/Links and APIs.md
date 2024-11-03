# PDOK Luchtfoto RGB (Open)
[https://www.pdok.nl/introductie/-/article/pdok-luchtfoto-rgb-open-](https://www.pdok.nl/introductie/-/article/pdok-luchtfoto-rgb-open-)

### API
- Service type: WMTS

- url for GetCapabilities: https://service.pdok.nl/hwh/luchtfotorgb/wmts/v1_0?request=GetCapabilities&service=WMTS

- Get tile request: 
```
https://service.pdok.nl/hwh/luchtfotorgb/wmts/v1_0?
service=WMTS
&request=GetTile
&version=1.0.0
&layer=luchtfotorgb
&style=default
&tilematrixset=EPSG:28992
&tilematrix=10
&tilerow=215
&tilecol=362
&format=image/png
```

### For Leaflet usage: 
```
L.tileLayer('https://service.pdok.nl/hwh/luchtfotorgb/wmts/v1_0?', {
    layer: 'luchtfotorgb',
    tilematrixSet: 'EPSG:28992',
    format: 'image/png',
    style: 'default',
    zoomOffset: -1,
}).addTo(map);
```


# PDOK Luchtfoto Infrarood (Open)
[https://www.pdok.nl/introductie/-/article/pdok-luchtfoto-infrarood-open-](https://www.pdok.nl/introductie/-/article/pdok-luchtfoto-infrarood-open-)

### API
- Service type: WMTS

- url for GetCapabilities: https://service.pdok.nl/hwh/luchtfotocir/wmts/v1_0?request=GetCapabilities&service=WMTS

- Get tile request:
 
```
https://service.pdok.nl/hwh/luchtfotocir/wmts/v1_0?
service=WMTS
&request=GetTile
&version=1.0.0
&layer=luchtfotocir
&style=default
&tilematrixset=EPSG:28992
&tilematrix=10
&tilerow=215
&tilecol=362
&format=image/png
```

### For Leaflet usage: 
```
L.tileLayer('https://service.pdok.nl/hwh/luchtfotocir/wmts/v1_0?', {
    layer: 'luchtfotocir',
    tilematrixSet: 'EPSG:28992',
    format: 'image/png',
    style: 'default',
    zoomOffset: -1,
}).addTo(map);
```


# Actueel Hoogtebestand Nederland (AHN)
[https://www.pdok.nl/introductie/-/article/actueel-hoogtebestand-nederland-ahn](https://www.pdok.nl/introductie/-/article/actueel-hoogtebestand-nederland-ahn)

### API
- Service type: WCS

- url for GetCapabilites: https://service.pdok.nl/rws/ahn/wcs/v1_0?service=WCS&request=GetCapabilities

- url for DescribeCoverage: https://service.pdok.nl/rws/ahn/wcs/v1_0?service=WCS&request=DescribeCoverage&version=1.0.0&coverage=ahn3_05m_dtm <!-- Replace ahn3_05m_dtm with the name of the coverage (this will be listed in the GetCapabilities response) -->

- GetCoverage request: 
```
https://service.pdok.nl/rws/ahn/wcs/v1_0?
service=WCS
&request=GetCoverage
&version=1.0.0
&coverage=ahn3_05m_dtm
&crs=EPSG:28992
&bbox=160000,400000,170000,410000
&width=1000
&height=1000
&format=image/geotiff
```
<!-- This request will provide you with a GeoTIFF file for the specified bounding box in the RD (EPSG:28992) coordinate system. -->


# Kadastrale kaart
[https://www.pdok.nl/introductie/-/article/kadastrale-kaart](https://www.pdok.nl/introductie/-/article/kadastrale-kaart)

### API
- Service type: WFS

- Layer name: kadastralekaart:Perceel, kadastralekaart:KadastraleGrens, kadastralekaart:Nummeraanduidingreeks, kadastralekaart:Bebouwing, kadastralekaart:OpenbareRuimteNaam

- url for GetCapabilities: https://service.pdok.nl/kadaster/kadastralekaart/wfs/v5_0?service=WFS&request=GetCapabilities


- url for DescibeFeatureType: https://service.pdok.nl/kadaster/kadastralekaart/wfs/v5_0?service=WFS&request=DescribeFeatureType&version=1.0.0&typeName=kadastralekaart:Perceel 
<!-- Replace kadastralekaart:Perceel with the desired feature type from the GetCapabilities response. -->




# 
[]()

### API
- Service type: WMTS
- url: 
- Layer name: 





# 
[]()

### API
- Service type: WMTS
- url: 
- Layer name: 
