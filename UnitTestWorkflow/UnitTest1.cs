﻿using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NeuroSystem.Workflow.Core.Extensions;
using NeuroSystem.Workflow.Core.Process;
using NeuroSystem.Workflow.Engine;
using NeuroSystem.Workflow.Engine.DAL;
using NeuroSystem.Workflow.UI.Consola;
using UnitTestWorkflow.Processes;

namespace UnitTestWorkflow
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
            var processId = Guid.NewGuid();
            var newProcess = new HelloWorldProcess() { Id = processId };

            var dal = new WorkflowEngineDAL();
            var engine = new WorkflowEngine(dal);

            engine.AddProcess(newProcess);
            engine.RunOneIteration();
            var process = engine.GetProcess(processId);

            Assert.AreEqual("Hello World", process.ProcesOutput);
        }

        [TestMethod]
        public void TestMethod2()
        {
            var processId = Guid.NewGuid();
            var newProcess = new TestConsoleProcess() {Id = processId };
            
            var dal = new WorkflowEngineDAL();
            var engine = new WorkflowEngine(dal);

            //Running proces in engine
            engine.AddProcess(newProcess);


            engine.RunOneIteration(); //powinien wyśweitlić dane 'Enter name'
            Assert.AreEqual(engine.GetProcessStatus(processId), EnumProcessStatus.WaitingForUserData);
            engine.SetUserData(processId, null); //nic nie podaje - to było tylko wyświetlenie
            var ser = dal.GetSerializedProcess(processId);
            var rozmiar = ser.VirtualMachineXml.Length;
            var vm = ser.GetVirtualMachine();
            var process = vm.GetProcess();


            engine.RunOneIteration(); //powinien poprosić od wczytanie danych z klawiatury
            Assert.AreEqual(engine.GetProcessStatus(processId), EnumProcessStatus.WaitingForUserData);
            engine.SetUserData(processId, new ConsoleRead() {Text = "Tomek"});
            ser = dal.GetSerializedProcess(processId);
            rozmiar = ser.VirtualMachineXml.Length;
            vm = ser.GetVirtualMachine();
            process = vm.GetProcess();


            engine.RunOneIteration(); //powinien wyśweitlić dane 'Enter age'
            Assert.AreEqual(engine.GetProcessStatus(processId), EnumProcessStatus.WaitingForUserData);
            engine.SetUserData(processId, null); //nic nie podaje - to było tylko wyświetlenie
            ser = dal.GetSerializedProcess(processId);
            rozmiar = ser.VirtualMachineXml.Length;
            vm = ser.GetVirtualMachine();
            process = vm.GetProcess();


            engine.RunOneIteration(); //powinien poprosić od wczytanie danych z klawiatury
            Assert.AreEqual(engine.GetProcessStatus(processId), EnumProcessStatus.WaitingForUserData);
            engine.SetUserData(processId, new ConsoleRead() { Text = "30" });
            ser = dal.GetSerializedProcess(processId);
            rozmiar = ser.VirtualMachineXml.Length;
            vm = ser.GetVirtualMachine();
            process = vm.GetProcess();


            engine.RunOneIteration(); //powinien zakończyć proces
            Assert.AreEqual(engine.GetProcessStatus(processId), EnumProcessStatus.Executed);

            process = engine.GetProcess(processId);
            Assert.AreEqual("Hello Tomek 30", process.ProcesOutput);
        }
    }
}