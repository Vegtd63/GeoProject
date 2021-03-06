using System;
using System.Collections.Generic;

namespace GeoApp
{
    /// <summary>
    /// This class is the main logic of our program. A middle man between
    /// the repo class and the UI.
    /// </summary>
    /// @author: Demetrios Green, Ben Pink, Clayton Rath, David Vegter
    public class Controller : IController
    {
        private IRepository _repo;
        private string errorMessage;

        public Controller(IRepository repository)
        {
            _repo = repository;
        }

        public List<Sample> GetAllSamples()
        {
            return _repo.RetrieveAllSamples();
        }

        public Sample GetSampleById(int id)
        {
            return _repo.RetrieveSampleById(id);
        }

        public List<Sample> GetSamplesByKeyword(string keyword)
        {
            List<Sample> currentList = _repo.RetrieveAllSamples();
            List<Sample> returnList = new List<Sample>();
            for (int i = 0; i < currentList.Count; i++)
            {
                Sample currentSample = currentList[i];
                if (currentSample.SampleId.ToString().Contains(keyword))
                {
                    returnList.Add(currentSample);
                }
                else if (currentSample.Name.ToString().Contains(keyword))
                {
                    returnList.Add(currentSample);
                }
                else if (currentSample.SampleType.ToString().Contains(keyword))
                {
                    returnList.Add(currentSample);
                }
                else if (currentSample.LocationDescription.ToString().Contains(keyword))
                {
                    returnList.Add(currentSample);
                }
                else if (currentSample.GeologicAge.ToString().Contains(keyword))
                {
                    returnList.Add(currentSample);
                }
            }
            return returnList;
        }

        public Sample CreateNewSample(List<string> sampleInfo, byte[] image)
        {
            Sample sample = new();
            if (!int.TryParse(sampleInfo[0], out int sampleId))
            {
                _setErrorMessage("Invalid Format: Sample ID");
                return null;
            }
            else
            {
                sample.SampleId = sampleId;
                for (int i = 1; i <= 6; i++)
                {
                    if (sampleInfo[i].Length >= 255)
                    {
                        switch (i)
                        {
                            case 1:
                                _setErrorMessage("Invalid Format: Sample Name");
                                break;
                            case 2:
                                _setErrorMessage("Invalid Format: Sample Type");
                                break;
                            case 3:
                                _setErrorMessage("Invalid Format: Sample Geologic Age");
                                break;
                            case 4:
                                _setErrorMessage("Invalid Format: Sample Location Description");
                                break;
                            case 5:
                                _setErrorMessage("Invalid Format: Sample City");
                                break;
                            case 6:
                                _setErrorMessage("Invalid Format: Sample State");
                                break;
                            case 7:
                                _setErrorMessage("Invalid Format: Sample Country");
                                break;
                            default:
                                _setErrorMessage("Unknown Error: Please Contact Your System Admin");
                                break;
                        }
                        return null;
                    }
                }
            }
            sample.Name = sampleInfo[1];
            sample.SampleType = sampleInfo[2];
            sample.GeologicAge = sampleInfo[3];
            sample.LocationDescription = sampleInfo[4];
            sample.City = sampleInfo[5];
            sample.State = sampleInfo[6];
            sample.Country = sampleInfo[7];
            if (sampleInfo[8] != string.Empty)
            {
                if (!double.TryParse(sampleInfo[8], out double latitude))
                {
                    _setErrorMessage("Invalid Format: Sample Latitude");
                    return null;
                }
                else
                {
                    sample.Latitude = latitude;
                }

            }
            if (sampleInfo[9] != string.Empty)
            {
                if (!double.TryParse(sampleInfo[9], out double longitude))
                {
                    _setErrorMessage("Invalid Format: Sample Longitude");
                    return null;
                }
                else
                {
                    sample.Longitude = longitude;
                }

            }
            sample.Image = image;
            bool added = _repo.AddNewSample(sample);
            Sample newSample = null;
            if (added)
            {
                newSample = _repo.RetrieveSampleByFields(sample);
            }


            return added ? newSample : null;
        }

        public bool UpdateSample(List<string> sampleInfo, byte[] image)
        {
            Sample sample = new();
            if (!int.TryParse(sampleInfo[0], out int dbId))
            {
                _setErrorMessage("Unknown Error: Please Contact Your System Admin");
                return false;
            }
            else
            {
                sample.DbId = dbId;
            }

            if (!int.TryParse(sampleInfo[1], out int sampleId))
            {
                _setErrorMessage("Invalid Format: Sample ID");
                return false;
            }
            else
            {
                sample.SampleId = sampleId;
                for (int i = 2; i <= 8; i++)
                {
                    if (sampleInfo[i].Length >= 255)
                    {
                        switch (i)
                        {
                            case 2:
                                _setErrorMessage("Invalid Format: Sample Name");
                                break;
                            case 3:
                                _setErrorMessage("Invalid Format: Sample Type");
                                break;
                            case 4:
                                _setErrorMessage("Invalid Format: Sample Geologic Age");
                                break;
                            case 5:
                                _setErrorMessage("Invalid Format: Sample Location Description");
                                break;
                            case 6:
                                _setErrorMessage("Invalid Format: Sample City");
                                break;
                            case 7:
                                _setErrorMessage("Invalid Format: Sample State");
                                break;
                            case 8:
                                _setErrorMessage("Invalid Format: Sample Country");
                                break;
                            default:
                                _setErrorMessage("Unknown Error: Please Contact Your System Admin");
                                break;
                        }
                        return false;
                    }
                }
            }
            sample.Name = sampleInfo[2];
            sample.SampleType = sampleInfo[3];
            sample.GeologicAge = sampleInfo[4];
            sample.LocationDescription = sampleInfo[5];
            sample.City = sampleInfo[6];
            sample.State = sampleInfo[7];
            sample.Country = sampleInfo[8];
            if (sampleInfo[9] != string.Empty)
            {
                if (!double.TryParse(sampleInfo[9], out double latitude))
                {
                    _setErrorMessage("Invalid Format: Sample Latitude");
                    return false;
                }
                else
                {
                    sample.Latitude = latitude;
                }

            }

            if (sampleInfo[10] != string.Empty)
            {
                if (!double.TryParse(sampleInfo[10], out double longitude))
                {
                    _setErrorMessage("Invalid Format: Sample Longitude");
                    return false;
                }
                else
                {
                    sample.Longitude = longitude;
                }

            }
            if (image != null)
            {
                sample.Image = image;
            }

            return _repo.EditSampleById(sample);
        }

        public bool DeleteSample(Sample sample)
        {
            return _repo.DeleteSampleById(sample.DbId);
        }

        public List<Issue> GetAllIssues()
        {
            return _repo.RetrieveAllIssues();
        }

        public bool CreateIssue(List<string> issueInfo)
        {
            Issue issue = new Issue();
            if (issueInfo[0].Equals("0"))
            {
                issue.Type = Issue.IssueType.Misinformation;
            }
            else
            {
                issue.Type = Issue.IssueType.SystemIssue;
            }
            if (issueInfo[1].Length > 500)
            {
                return false;
            }
            else
            {
                issue.IssueDescription = issueInfo[1];
            }
            issue.DateTimeSubmitted = DateTime.Now;
            issue.Resolved = false;

            return _repo.CreateIssue(issue);
        }

        public bool DeleteIssue(Issue issue)
        {
            return _repo.DeleteIssueById(issue.referenceId);
        }

        public string getErrorMessage()
        {
            return errorMessage;
        }

        private void _setErrorMessage(string errorMessage)
        {
            this.errorMessage = errorMessage;
        }
    }
}
