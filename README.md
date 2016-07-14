ECRComms
========

Sams4s communications library and tool, alpha code

This library is for communicating with Sams4S ER380M and ER-230 cash registers

At this present moment it is only fully working with the ER380M (UK) versions.

As far as I can tell the biggest changes between the different versions of ER380 
and even the ER-230 is the PLU file format as the exact length seems to change
and hold slightly different bits of info.

The library is split into a number of parts, the main part handles 
communications with the ECR, the communications protocol seems idential between 
the different eeprom versions and even across a few diffrent models. At this 
level the library does 3 basic tasks.

1) Upload a data file
2) Download a data file
3) Get a report

All three functions need to encode the data and escape/unescape special 
characters etc and the library takes care of that as well as checksums. These 
functions appear to work well. But to fully use the communications functions 
you need to understand the data files and report formats.

The library also includes code to create and decode the various data files and 
reports and currently supports the following data files :-

PLU File (ER380M UK ONLY) (ER-230 code currently not finished)
Stock File (all versions seem the same)
Descriptor File (all versions seem the same)

it also supports the following reports :-

PLU report (stock change report)
Financial report (X1,X2,Z1,Z2)

----------------------------------------------------

I am planning on supporting the full range of data files and the full range
of reports. I am also hoping to fully support the ER-380M (UK) and the ER-230
as i have both of these ECRs, if anyone wants to help decode the PLU format
for other versions of the ER-380 they are more than welcome to help.

Currently the library although alpha code, is being used in anger as part of a
stock control system using the PLU Files upload to add data to the ECR and the
PLU report to get the daily sales.


The required files for basic operation are:-

ECRComms.cs (the main bit)
PLU.cs (if you want to process PLU files)
Stock.cs (if you want to process stock reports)
Reports.cs (will decode some reports)

Other files in tidy/ are not required as progress has no place to be in this
lib as its a GUI component and the Datafiles in properties/ are unfinished work
where i fleshed out each report format.

So just build those 4 either as a lib or with your main program

Example usage
---------------------

public static ECRComms ecr;
ecr = new ECRComms();
ecr.commport = "/dev/ttyUSB0"; //""/dev/ttyUSB0""
ecr.baud = 9600;
bool success = ecr.init();

if(success==false)
{
    Console.WriteLine("Failed to open ECR :-(");
    return;
}

List<byte> payload;
payload = ecr.getreport(ECRComms.reports.PLU, ECRComms.reporttype.Z1);

p = new PLUReport(payload.ToArray());
p.savetofile(path);

foreach (PLUReportEntry e in p.entries)
{
    Console.WriteLine("We sold {0} of
{1}",e.quantity,e.PLU.code.ToString());
}


That will grab the Z1 PLU report(this is just a list of barcodes/PLUs
and the amount sold) so very useful for stock reports

All other reports work in the same way, you use getreport() and the
type of report()


Example PLU upload
-----------------------------

Warning ensure the PLU format matches your ECR eeprom or you will end
up with a big mess and have to reset the ECR and loose any programming
you may have.

If you are unsure first program some PLUs via the keyboard of the ECR,
then following the example above DOWNLOAD the PLU file from the ECR
and sent it to me robin.cornelius@gmail.com for analysis


-----------------------------

ecr = new ECRComms();
ecr.commport = "/dev/ttyUSB0"; //""/dev/ttyUSB0""
ecr.baud = 9600;
bool success = ecr.init();

if (success == false)
{
    Console.WriteLine("Failed to open ECR :-(");
    return;
}

List<data_serialisation> ds = new List<data_serialisation>();

ER380M_PLU p = new ER380M_PLU();
p.PLUcode = new barcode();
p.PLUcode.fromtext("01234567"); //this is your barcode/plu/sku codes
p.PLUcode.encode();

p.price = price;
p.description = "Name of stock" //first 12 letters appear on recipt
p.encode();

ds.Add(p);

//add as many plus as you want to ds

 ecr.setprogram(ECRComms.program.PLU, ds);


