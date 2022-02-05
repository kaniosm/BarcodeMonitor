# BarcodeMonitor
Windows tray application that monitors a barcode scanner and replaces the barcodes with the corresponding item code


The purpose of this project is to replace the barcode coming from the scanner with the corresponding item code for applications that do support searching products by barcode.

- Windows app running in Tray
- Self add in Windows startup
- User Raw Input windows API to recieve global keyboard events - for Keyboard emulating scanners
- Use the serial port to monitor scanner - for COM Port mapped USB or COM scanners
- Use SendKeys method to delete the scanned barcode and send the keystrokes to enter the item code
- Load the Barcode/ItemCode mapping from an XML files (Can be replaced with database queries)
