#define normalMode //used for testing of configuration files using testConfig.cs
using System;
using System.Threading;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using  System.IO;
using  System.Reflection;
using  System.CodeDom.Compiler;
using  Microsoft.CSharp;
//using Microsoft.DirectX.DirectInput;

namespace SBC
{
	public partial class Form1 : Form
	{
		Object CSharpObject;
		Worker workerObject;
		Thread workerThread;
        CompilerParameters compilerParams;
		bool ProgramStarted = false;
        bool firstTime = true;
		
		public Form1(string[] args)
		{
			InitializeComponent();
		}

        private void checkvJoyProperties()
        {
            vJoy joystick = new vJoy();
            string installedVersion;
            // Get the driver attributes (Vendor ID, Product ID, Version Number)
            if (!joystick.isEnabled())
            {
                MessageBox.Show("vJoy driver not enabled: Failed Getting vJoy attributes.\n");
            }
            else
            {
                installedVersion = joystick.getSerialNumberString();
                if (installedVersion != joystick.getCompiledVersionNumber())
                    MessageBox.Show("vJoy version :" + installedVersion + "does not match required version of 2.1.5");
            }
            //Console.WriteLine("Vendor: {0}\nProduct :{1}\nVersion Number:{2}\n", joystick.GetvJoyManufacturerString(), joystick.GetvJoyProductString(), joystick.GetvJoySerialNumberString());
            

            int a = 1;
        }

		private void Form1_Load(object sender, EventArgs e)
		{
            checkvJoyProperties();

            fileString.Text = Properties.Settings.Default.storedFileName;
            //compilerParams.ReferencedAssemblies.Add("SBC.dll");
            //compilerParams.ReferencedAssemblies.Add("myVJoyWrapper.dll");
            //compilerParams.ReferencedAssemblies.Add("Microsoft.DirectX.DirectInput.dll");
		}

		private void timer1_Tick(object sender, EventArgs e)
		{
		   /* if (joystick.IsDeviceAccessible())
				joystick.UpdateJoystick();*/
		}

		private void Form1_FormClosing(object sender, FormClosingEventArgs e)
		{
			//joystick.Dispose();
            if (ProgramStarted)
            {
                workerObject.RequestStop();
                // Use the Join method to block the current thread 
                // until the object's thread terminates.
                workerThread.Join();
                Status.Text = "Stopped";
                ProgramStarted = false;
            }
		}

		private void StartBtn_Click(object sender, EventArgs e)
        {
			if(!ProgramStarted)
			{
			    CSharpCodeProvider codeProvider = new CSharpCodeProvider();
			    codeProvider.CreateCompiler();
			    //add compiler parameters

                compilerParams = new CompilerParameters();
                //compilerParams.ReferencedAssemblies.Add("Microsoft.DirectX.DirectInput.dll");
                foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
                {
                    compilerParams.ReferencedAssemblies.Add(assembly.Location);
                }

                String fileSource = File.ReadAllText(fileString.Text);
                //make .cs files compatible with old code
                fileSource = fileSource.Replace("Microsoft.DirectX.DirectInput.Key", "SBC.Key");
                
                CompilerResults results = codeProvider.CompileAssemblyFromSource(compilerParams, fileSource);
			    if (results.Errors.Count > 0)
			    {
				    //MessageBox.Show("There were errors");
				    int i = 0;
				    string[] newLines = new string[results.Errors.Count * 2];
				    foreach (CompilerError error in results.Errors)
				    {
					    newLines[i] = error.ToString();
					    newLines[i + 1] = "\n";
					    i = i + 2;
				    }
				    errorBox.Lines = newLines;
			    }
			    else
			    {
                    firstTime = false;//once we create a sucessful assembly, we don't need to keep referencing DirectX.Direcinput
                    errorBox.Lines  = null;
                    CSharpObject = results.CompiledAssembly.CreateInstance("SBC.DynamicClass");

				    // Create the thread object. This does not start the thread.
				    workerObject = new Worker(ref CSharpObject,this);
                    //workerObject = new Worker(this);
				    workerThread = new Thread(workerObject.DoWork);
                    Status.Text = "Running";
				    ProgramStarted = true;

				    // Start the worker thread.
                    try
                    {
                        workerThread.Start();
                    }
                    catch(Exception ex)
                    {
                        MessageBox.Show(ex.ToString());
                    }

			    }
			}

		}

		private void StopBtn_Click(object sender, EventArgs e)
		{
            if (ProgramStarted)
            {
                workerObject.RequestStop();
                // Use the Join method to block the current thread 
                // until the object's thread terminates.
                workerThread.Join();

                Status.Text = "Stopped";
                ProgramStarted = false;
            }
		}

        public delegate void SetControlPropertyThreadSafeDelegate(Control control, string propertyName, object propertyValue);
        public static void SetControlPropertyThreadSafe(Control control, string propertyName, object propertyValue)
        {
            if (control.InvokeRequired)
            {
                control.Invoke(new SetControlPropertyThreadSafeDelegate(SetControlPropertyThreadSafe), new object[] { control, propertyName, propertyValue });
            }
            else
            {
                control.GetType().InvokeMember(propertyName, BindingFlags.SetProperty, null, control, new object[] { propertyValue });
            }
        }

        public void updateDebugString(String text)
        {
            string[] newLines = text.Split(new string[] { "\n" }, StringSplitOptions.None);
            // thread-safe equivalent of
            // myLabel.Text = status;
            SetControlPropertyThreadSafe(errorBox, "Lines", newLines);
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Displays an OpenFileDialog so the user can select a Cursor.
            OpenFileDialog openFileDialog1 = new OpenFileDialog();
            openFileDialog1.Filter = "C# files|*.cs";
            openFileDialog1.Title = "Select a C# configuration File";

            // Show the Dialog.
            // If the user clicked OK in the dialog and
            // a .CUR file was selected, open it.
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                // Assign the cursor in the Stream to the Form's Cursor property.
                Properties.Settings.Default.storedFileName = fileString.Text = openFileDialog1.FileName;
                Properties.Settings.Default.Save();
            }
        }

        private void aboutToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
AssemblyCopyrightAttribute copyright =
(AssemblyCopyrightAttribute)AssemblyCopyrightAttribute.GetCustomAttribute(
assembly, typeof( AssemblyCopyrightAttribute ) );
AssemblyTitleAttribute title =
(AssemblyTitleAttribute)AssemblyTitleAttribute.GetCustomAttribute(
assembly, typeof( AssemblyTitleAttribute ) );
System.IO.FileInfo info = new System.IO.FileInfo( assembly.Location );
DateTime date = info.LastWriteTime;

MessageBox.Show(
title.Title  +
" version " + 
assembly.GetName().Version.ToString() + "\n" +
"released " +
date.ToShortDateString() +
"\nWritten by Santiago Saldana." + "\n" +
"For the latest version, visit http://code.google.com/p/steel-batallion-64/", "About Steel-Batallion-64", MessageBoxButtons.OK, MessageBoxIcon.None, MessageBoxDefaultButton.Button2, 0, "http://code.google.com/p/steel-batallion-64/w/list");
        }
	}
	public class Worker
	{
		Object CSharpObject;
        Form1 myForm;
        public Worker(Form1 form)
        {
            myForm = form;
        }
		public Worker(ref Object anObject, Form1 form)
		{
			CSharpObject = anObject;
            myForm = form;
		}
		// This method will be called when the thread is started.
		public void DoWork()
		{
#if normalMode
            try
            {
                CSharpObject.GetType().InvokeMember("Initialize", System.Reflection.BindingFlags.InvokeMethod, null, CSharpObject, null);
                string[] debugLines = new string[1];

		        int refreshRate = (int)CSharpObject.GetType().InvokeMember("getRefreshRate", System.Reflection.BindingFlags.InvokeMethod, null, CSharpObject, null);
		        while (!_shouldStop)
		        {
			        CSharpObject.GetType().InvokeMember("mainLoop", System.Reflection.BindingFlags.InvokeMethod, null, CSharpObject, null);
                    if (CSharpObject.GetType().GetMethod("getDebugString") != null)
                    {
                        String aString = (String)CSharpObject.GetType().InvokeMember("getDebugString", System.Reflection.BindingFlags.InvokeMethod, null, CSharpObject, null);
                        //String aString;
                        myForm.updateDebugString(aString);
                    }
			        Thread.Sleep(refreshRate);
		        }
		        CSharpObject.GetType().InvokeMember("shutDown", System.Reflection.BindingFlags.InvokeMethod, null, CSharpObject, null);
                return;

            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
            }
#else
            testConfig config;
            config = new testConfig();
            try
            {
                config.Initialize();
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
            }
            
            int refreshRate = config.getRefreshRate();
            while (!_shouldStop)
            {
                config.mainLoop();
                Thread.Sleep(refreshRate);
            }
             
            config.shutDown();

            return;
#endif

        }
		public void RequestStop()
		{
			_shouldStop = true;
		}
		// Volatile is used as hint to the compiler that this data
		// member will be accessed by multiple threads.
		private volatile bool _shouldStop;
	}
}
