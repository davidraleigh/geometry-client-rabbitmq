# geometry-client-rabbitmq
A demonstration of how one could write a client that will interact with the rabbitmq [geometry-worker](http://github.com/davidraleigh/geometry-worker) that powers http://geometry.fogmodel.io.

On line 60 of the test app there is a `testGeomString` variable. If you go to http://geometry.fogmodel.io you can get a sample of different types of operators and the message format for sending requests to the worker. For instance if you click the `Show JSON` button on the webpage you can then copy the `JSON Request` field and place it into your application

Appearance of JSON Request on page http://geometry.fogmodel.io after clicking Show JSON:
```javascript
{
	"operator_name": "Union",
	"left_wkt_geometries": [
		"MULTIPOLYGON (((40 40, 20 45, 45 30, 40 40)), ((20 35, 45 20, 30 5, 10 10, 10 30, 20 35), (30 20, 20 25, 20 15, 30 20)))",
		"POLYGON ((30 10, 40 40, 20 40, 10 20, 30 10))",
		"MULTIPOLYGON (((30 20, 45 40, 10 40, 30 20)),((15 5, 40 10, 10 20, 5 10, 15 5)))"
	]
}
```

Clipboard version that can be copied into the geometry-client `Send.cs` file at line 60:
``` c#
{\n\t\"operator_name\": \"Union\",\n\t\"left_wkt_geometries\": [\n\t\t\"MULTIPOLYGON (((40 40, 20 45, 45 30, 40 40)), ((20 35, 45 20, 30 5, 10 10, 10 30, 20 35), (30 20, 20 25, 20 15, 30 20)))\",\n\t\t\"POLYGON ((30 10, 40 40, 20 40, 10 20, 30 10))\",\n\t\t\"MULTIPOLYGON (((30 20, 45 40, 10 40, 30 20)),((15 5, 40 10, 10 20, 5 10, 15 5)))\"\n\t]\n}
```

The response returned by the geometry-client-rabbitmq application will match the `JSON Results` field on the http://geometry.fogmodel.io page (remember to click `Show JSON` to demonstrate JSON messages).