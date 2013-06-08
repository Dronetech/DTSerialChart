Dronetech Serial Visualizer
===========

### Description

This application reads a Serial Port for a csv string and parse it to a chart. 

The chart series are customized and it can save the chart in a png file for future reference.

All data from serial port is saved in a textbox so it can be exported after to excel or other program for further analysis. 

---

### Dependencies

* .Net Framework 4.0
* Visual Studio 2010

### Example usage

* Prepare an arduino board with a example sketch that produces a csv serial stream
* Connect a UART adapter to the USB port (http://www.dronetech.eu/store/index.php?route=product/product&product_id=53)
* Connect the TX arduino to the RX of the adapter
* Select port, baud, interval and series format.
* Press the button and watch the chart go

### Series format

The series are generated based on a csv with the following format:

<Index>|<title 1>,<Index>|<title 2>

Index is the position on the csv stream that Y axes data is and the title of the Series.

Example: 
csv stream 				-> 123,340
series format string 	-> 0|Sensor A, 1|Sensor B
Result:
Series 1 title = Sensor A 
Serias 1 Y Axes value = 123
Series 2 title = Sensor B 
Serias 2 Y Axes value = 340


    
