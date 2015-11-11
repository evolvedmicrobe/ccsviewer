using System;
using Gtk;
using System.IO;
using System.Linq;
using System.Collections.Generic;

using Bio;
using Bio.BWA;
using Bio.IO;
using Bio.Variant;
using PacBio.BAM;


namespace ccsviewer
{
    class MainClass
    {
        public static void Main (string[] args)
        {
            try {
                PlatformManager.Services.MaxSequenceSize = int.MaxValue;
                PlatformManager.Services.DefaultBufferSize = 4096;
                PlatformManager.Services.Is64BitProcessType = true;

                if (args.Length > 4) {
                    Console.WriteLine ("Too many arguments");
                    DisplayHelp ();
                } else if (args.Length < 4) {
                    Console.WriteLine ("Not enough arguments");
                    DisplayHelp();
                }else if (args [0] == "h" || args [0] == "help" || args [0] == "?" || args [0] == "-h") {
                    DisplayHelp ();
                } else {
                    
                    string subreads_name = args [0];
                    string ccs_name = args [1];
                    string ref_name = args [2];
                    string zmw = args[3];

                    // Validate files exist
                    for(int i = 0; i < 3; i++) {
                        var fname = args[i];
                        if (!File.Exists(fname)) {
                            Console.WriteLine ("Can't find file: " + fname);
                            return;
                        }
                        if (i < 2) {
                            //TODO: Add code to ensure BAM exists here
                        }
                    }

                    // Validate ZMW Number
                    int holeNumber;
                    bool converted = int.TryParse(zmw, out holeNumber);
                    if (!converted || holeNumber < 0) {
                        Console.WriteLine("Could not convert " + zmw +" into hole number >= 0");
                    }

                    Console.WriteLine("Loading Data ...");

                    // Get CCS Read
                    IntList zmws = new IntList();
                    zmws.Add(holeNumber);
                    //zmws.Add(133403);
                    DataSet dt = new DataSet(ccs_name);
                    var query = new ZmwQuery(zmws, dt);
                    var ccs = query.FirstOrDefault();
                    if (ccs == null) {
                        Console.WriteLine("Could not query hole number " + holeNumber + " from file " + ccs_name);
                        return;
                    }
                    var seq_str = ccs.Sequence();
                    var sequence = new Sequence(DnaAlphabet.Instance, seq_str);
                    sequence.ID = ccs.FullName();
                    //Clean up
                    query.Dispose(); query = null;
                    ccs.Dispose(); ccs = null;
                    dt.Dispose(); dt = null;

                    Console.WriteLine("Aligning Data ... ");
                    BWAPairwiseAligner bwa = new BWAPairwiseAligner(ref_name, false); 

                    // Now get initial alignment and variants
                    BWAPairwiseAlignment aln =  bwa.AlignRead(sequence) as BWAPairwiseAlignment;
                    if (aln == null) {
                        Console.WriteLine("Consensus read did not align");
                        return;
                    }

                    // Now get all the subreads
                    dt = new DataSet(subreads_name);
                    query = new ZmwQuery(zmws, dt);
                    // We must copy the data right away, or we wind up with a bunch of pointers that all hit the same thing
                    var subreads = query.Select(s => new Sequence(DnaAlphabet.Instance, s.Sequence()) {ID = s.FullName()}).ToList();
                    if (subreads.Count == 0 )
                    {
                        Console.WriteLine("Did not find any subreads from " + holeNumber + " in file " + subreads_name);
                        return;
                    }
                    subreads.ForEach( s => { 
                        var split = s.ID.Split('/');
                        s.ID = String.Join("/", split.Skip(1)); });
                    
                    zmws.Dispose(); zmws = null;
                    query.Dispose(); query = null;

                   
                    // Now generate the dataset
                    var data = new CCSDataSet(sequence, subreads, aln);




                    Application.Init ();
                    MainWindow win = new MainWindow (data);
                    win.Show ();
                    Application.Run ();

                }


            }
            catch(DllNotFoundException thrown) {
                Console.WriteLine ("Error thrown when attempting to generate the CCS results.");
                Console.WriteLine("A shared library was not found.  To solve this, please add the folder" +
                    " with the downloaded files *.so and *.dylib " +
                    "to your environmental variables (LD_LIBRARY_PATH on Ubuntu, DYLD_LIBRARY_PATH on Mac OS X)."); 
                Console.WriteLine ("Error: " + thrown.Message);
                Console.WriteLine (thrown.StackTrace);

            }
            catch(Exception thrown) {
                Console.WriteLine ("Error thrown when attempting to generate the CCS results");
                Console.WriteLine ("Error: " + thrown.Message);
                Console.WriteLine (thrown.StackTrace);
                while (thrown.InnerException != null) {
                    Console.WriteLine ("Inner Exception: " + thrown.InnerException.Message);
                    thrown = thrown.InnerException;
                }
            }
          
        }
        static void DisplayHelp() {
            Console.WriteLine ("ccsviewer SUBREADS.BAM CCS.BAM REF ZMW");
            Console.WriteLine ("SUBREADS.BAM - the original subreads file");
            Console.WriteLine ("CCS.BAM - the ccs bam file");
            Console.WriteLine ("REF - A fasta file with the references");
            Console.WriteLine ("ZMW - The ZMW to look at and examine");

        }
    }
}
