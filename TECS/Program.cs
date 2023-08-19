using System;
using TECS.HDLSimulator;

namespace TECS;

public static class Program
{
     public static void Main(string[] args)
     {
          new Simulator(Settings.DataFolder);
     }
}