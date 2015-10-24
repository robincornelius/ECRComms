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



