using System;
using Gtk;
using Pango;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using TestApp;

public partial class MainWindow: Gtk.Window
{
    public int maxLength = 500;
    int alnLength = 40;
    TextTag A;
    TextTag C;
    TextTag G;
    TextTag T;

    public MainWindow () : base (Gtk.WindowType.Toplevel)
    {
        MSA msa = new MSA ();
        Build ();
        //createBelvuAlignment ();

        A = CreateTag ("red", "black");
        C = CreateTag ("blue", "white");
        G = CreateTag ("yellow", "black");
        T = CreateTag ("green","white");
       // this.table2.ModifyFont (FontDescription.FromString ("Courier"));
       // this.textview1.fon
        drawAlignment(msa);
    }

    public void createBelvuAlignment() {
//        var seqArea = new Gtk.Layout (IntPtr.Zero);
//        var columnsArea = new Gtk.Layout (IntPtr.Zero);
//        var columnsHeader = new Gtk.DrawingArea ();
//        var seqHeader = new Gtk.DrawingArea ();
//        uint xpad = 0;
//        uint ypad = 0;
//
//        this.table1.Attach (columnsHeader, 0, 1, 0, 1, AttachOptions.Fill, AttachOptions.Shrink, xpad, ypad);
//        this.table1.Attach (columnsArea as Gtk.Widget, 0, 1, 1, 2, AttachOptions.Fill, AttachOptions.Expand | AttachOptions.Fill, xpad, ypad);
//        this.table1.Attach (seqHeader, 1, 2, 0, 1, AttachOptions.Fill, AttachOptions.Fill, xpad, ypad);
//        this.table1.Attach (seqArea, 1, 2, 1, 2, AttachOptions.Fill | AttachOptions.Expand, AttachOptions.Fill | AttachOptions.Expand, xpad, ypad);
//
//        var hScrollBar = new Gtk.HScrollbar(new Adjustment(0, 0, maxLength, 5, 0, 0));
//        var vScrolBar = new VScrollbar (new Adjustment (0, 0, alnLength + 1, 1, 0, 0));
//
//        this.table1.Attach (hScrollBar, 1, 2, 2, 3, AttachOptions.Fill | AttachOptions.Expand, AttachOptions.Shrink, xpad, ypad);
//        this.table1.Attach (vScrolBar, 2, 3, 1, 2, AttachOptions.Shrink, AttachOptions.Fill | AttachOptions.Expand, xpad, ypad);

            

    }

    private void drawAlignment(MSA aln) {

        // Write sequence names first
        var names = String.Join ("\n", aln.Sequences.Select (z => z.Name));
        this.textview_seqnames.Buffer.Text = names;

        var seqs = String.Join ("\n", aln.Sequences.Select (z => z.Sequence));
        this.textview_seqs.Buffer.Text = seqs;
        var iter = this.textview_seqs.Buffer.GetIterAtLine (0);
        int pos = 0;
        int line = 0;
        do {
            var bp = iter.Char;
            TextTag tag = null;
            switch (bp) {
            case "A":
                tag = A;
                break;
            case "C":
                tag = C;
                break;
            case "G":
                tag = G;
                break;
            case "T":
                tag = T;
                break;
            case "\n":
                line++;
                pos = 0;
                break;
            }
            if (tag != null) {
                textview_seqs.Buffer.ApplyTag (tag, iter, textview_seqs.Buffer.GetIterAtLineOffset (line, ++pos));
            }
        } while(iter.ForwardChar ());
    }



        
    TextTag CreateTag(string backgroundColor, string foregroundColor) {
        TextTag tag = new TextTag (null);
        //tag.Weight = Pango.Weight.Bold;
        tag.Background = backgroundColor;
        tag.Font = "Courier 14";
        tag.Foreground = foregroundColor;

        textview_seqs.Buffer.TagTable.Add (tag);
        return tag;
    }

    protected void OnDeleteEvent (object sender, DeleteEventArgs a)
    {
        Application.Quit ();
        a.RetVal = true;
    }


    protected void activateSignal (object sender, EventArgs e)
    {
        /*
        var iter = this.table2.Buffer.GetIterAtLine (0);
        int pos = 0;
        int line = 0;
        bool haveIter = true;
        do {
            var bp = iter.Char;
            TextTag tag = null;
            switch (bp) 
            {
                case "A":
                    tag = A;
                    break;
                case "C":
                    tag = C;
                    break;
                case "G":
                    tag = G;
                    break;
                case "T":
                    tag = T;
                    break;
            }
            if (tag !=null) {
                table2.Buffer.ApplyTag(tag, iter, this.table2.Buffer.GetIterAtLineOffset(line, ++pos));
            }
        } while(iter.ForwardChar());
       */
//        tag.Weight = Pango.Weight.Bold;
//        tag.Background = "red";
//
//        var iter = this.textview1.Buffer.GetIterAtLine (0);
//        this.textview1.Buffer.InsertWithTags (ref iter, "Bold text\n", tag);
//
//        FileChooserDialog fileChooser = new FileChooserDialog ("Choose an Image to View", this, FileChooserAction.Open, "Cancel", ResponseType.Cancel, "Open", ResponseType.Accept);
//        if (fileChooser.Run () == (int)ResponseType.Accept) {
//
//            if(fileChooser.Filename.ToLower().EndsWith(".png") || 
//                fileChooser.Filename.ToLower().EndsWith(".gif") ||
//                fileChooser.Filename.ToLower().EndsWith(".jpg"))
//            {
////                //Dispose of Old PixBuf
////                if (displayImage != null && displayImage.Pixbuf != null) {
////                    displayImage.Pixbuf.Dispose ();
////                }
////
////                displayImage.Pixbuf = new Gdk.Pixbuf (fileChooser.Filename);
//            }
//        }
//
//        fileChooser.Destroy ();

    }
   
}


    
   

