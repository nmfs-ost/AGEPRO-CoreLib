using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace Nmfs.Agepro.CoreLib
{
  /// <summary>
  /// General AGEPRO model Parameters
  /// </summary>
  public class AgeproGeneral
  {
    public int ProjYearStart { get; set; }    //First Year in Projection
    public int ProjYearEnd { get; set; }     //Last Year in Projection
    public int AgeBegin { get; set; }        //First Age Class
    public int AgeEnd { get; set; }           //Last Age Class
    public int NumFleets { get; set; }       //Number of Fleets
    public int NumRecModels { get; set; }    //Number of Recruit Models
    public int NumPopSims { get; set; }      //Number of Population Simulations
    public bool HasDiscards { get; set; }     //Discards are Present
    public int Seed { get; set; }            //Random Number Seed
    public string InputFile { get; set; }

    public AgeproGeneral()
    {

    }

    public AgeproGeneral(string file)
    {
      InputFile = file; //readin file contents
    }

    /// <summary>
    /// Determine number of years in projection by the (absolulte) diffefence between the 
    /// last and first year of projection. 
    /// </summary>
    /// <returns>The difference stored in 'nYears'</returns>
    public int NumYears()
    {
      return Math.Abs(ProjYearEnd - ProjYearStart) + 1;
    }

    /// <summary>
    /// Determine number of ages in projection by the (absolulte) diffefence between last age 
    /// class and first age class of projection. 
    /// </summary>
    /// <returns>The difference in stored in 'nAges'</returns>
    public int NumAges()
    {
      return Math.Abs(AgeBegin - AgeEnd) + 1;
    }

    /// <summary>
    /// Returns a sequence of years from First year of projection
    /// </summary>
    /// <returns>Returns a int array from <paramref name="projYearStart"/> by <paramref name="NumYears"/></returns>
    public int[] SeqYears()
    {
      return Enumerable.Range(ProjYearStart, NumYears()).ToArray();
    }

    /// <summary>
    /// Reads General AGEPRO parameters from Input File
    /// </summary>
    /// <param name="sr"></param>
    /// <returns></returns>
    public string ReadGeneralModelParameters(StreamReader sr)
    {
      if (sr is null)
      {
        throw new ArgumentNullException(nameof(sr));
      }

      string line = sr.ReadLine();
      string[] generalLine = line.Split(" ".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
      //Validate generalLine before assigning to agepro input file class model; if not, catch exception and display error message to user
      if (ValidateGeneralLineParameters(generalLine))
      {
        ProjYearStart = Convert.ToInt32(generalLine[0]);
        ProjYearEnd = Convert.ToInt32(generalLine[1]);
        AgeBegin = Convert.ToInt32(generalLine[2]);
        AgeEnd = Convert.ToInt32(generalLine[3]);
        NumPopSims = Convert.ToInt32(generalLine[4]);
        NumFleets = Convert.ToInt32(generalLine[5]);
        NumRecModels = Convert.ToInt32(generalLine[6]);
        Seed = Convert.ToInt32(generalLine[8]);
        HasDiscards = generalLine[7].Equals("1");
      }

      return line;
    }

    /// <summary>
    /// Writes General AGEPRO Model Parameers to Input File
    /// </summary>
    /// <returns></returns>
    public List<string> WriteAgeproGeneralParameters()
    {

      return new List<string>
      {
        "[GENERAL]",
        ProjYearStart.ToString() + "  " +
          ProjYearEnd.ToString() + "  " +
          AgeBegin.ToString() + "  " +
          AgeEnd.ToString() + "  " +
          NumPopSims.ToString() + "  " +
          NumFleets.ToString() + "  " +
          NumRecModels.ToString() + "  " +
          Convert.ToInt32(HasDiscards).ToString() + "  " +
          Seed.ToString()
      };
    }

    /// <summary>
    /// Helper function to validate the general line parameters read from the input file.  
    /// Checks for correct number of parameters, correct data types, and logical consistency between parameters. 
    /// If any validation fails, it will throw an appropriate error message.
    /// </summary>
    /// <param name="generalLine">String array containing the Agepro Model GENERAL settings</param>
    /// <returns>Returns boolean value TRUE if all checks are met.</returns>
    /// <exception cref="InvalidAgeproParameterException"></exception>
    private bool ValidateGeneralLineParameters(string[] generalLine)
    {
      if (generalLine.Length != 9)
      {
        throw new InvalidAgeproParameterException($"Invalid parameter count: {generalLine.Length}. Must have 9 parameters");
      }

      // Validate that all parameters in the generalLine are integers
      if(!generalLine.All(param => int.TryParse(param, out _)))
      {
        throw new InvalidAgeproParameterException("All values for general parameters must be integers");
      }

      // projYearStart cannot be greater than projYearEnd
      if (Convert.ToInt32(generalLine[0]) > Convert.ToInt32(generalLine[1]))
      {
        throw new InvalidAgeproParameterException("projYearStart cannot be greater than projYearEnd");
      }

      // ageBegin can only be 0 or 1
      if (Convert.ToInt32(generalLine[2]) < 0 || Convert.ToInt32(generalLine[2]) > 1)
      {
        throw new InvalidAgeproParameterException("ageBegin can only be 0 or 1");
      }

      // ageBegin cannot be greater than ageEnd
      if (Convert.ToInt32(generalLine[2]) > Convert.ToInt32(generalLine[3]))
      {
        throw new InvalidAgeproParameterException("ageBegin cannot be greater than ageEnd");
      }

      // numPopSims must be non-negative integers
      if (Convert.ToInt32(generalLine[4]) < 0)
      {
        throw new InvalidAgeproParameterException("numPopSims must be non-negative integers");
      }

      // numfleets must be non-nogative integer
      if (Convert.ToInt32(generalLine[5]) < 0)
      {
        throw new InvalidAgeproParameterException("numFleets must be a non-negative integer");
      }

      // numRecModels must be non-negative integer
      if (Convert.ToInt32(generalLine[6]) < 0)
      {
        throw new InvalidAgeproParameterException("numRecModels must be a non-negative integer");
      }

      //Seed can be negative integer, but not zero.
      if (!int.TryParse(generalLine[8], out int seed) || seed == 0)
      {
        throw new InvalidAgeproParameterException("Seed must be a non-zero integer");
      }

      //hasDiscards must be either 0 or 1 so that it can be converted to a boolean value
      if (!generalLine[7].Equals("0") && !generalLine[7].Equals("1"))
      {
        throw new InvalidAgeproParameterException("hasDiscards must be either 0 or 1");
      }


      return true;


    }
  }
}
