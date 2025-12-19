using FellowOakDicom;
using System;
using System.Collections.Generic;

namespace GodotDicomViewer.Code.Data
{
    // These objects will contain de Dicom data
    // They must be complete, all information of the data source most be stored
    // These objects are used in a cache

    // It is possible to derived for specific types of dicom modules, adding strongly types properties
    // The properties should return the value in types like string, DateTime, int. Ready to be used in a GUI.

    [Serializable]
    public abstract class BaseData : DicomDataset
    {
        public abstract string UID { get; }

        public Dictionary<string, string> GetPrettyDictionary()
        {
            var dict = new Dictionary<string, string>();
            foreach (var item in this)
            {
                // todo get readable name of dicom tag 
                dict.Add(item.Tag.ToString(), item.ToString());
            }
            return dict;
        }

        protected void copy_specific_tags(DicomDataset sourceDataset, DicomDataset destinationDataset, params DicomTag[] tagsToCopy)
        {
            foreach (var tag in tagsToCopy)
            {
                if (sourceDataset.Contains(tag))
                {
                    var element = sourceDataset.GetDicomItem<DicomElement>(tag);
                    if (destinationDataset.Contains(tag))
                    {
                        destinationDataset.Remove(tag);
                    }
                    destinationDataset.Add(element);
                }
            }
        }
       // public string SeriesInstanceUID { get { return GetSingleValueOrDefault(DicomTag.SeriesInstanceUID, ""); } }
        public string SOPInstanceUID { get { return GetSingleValueOrDefault(DicomTag.SOPInstanceUID, ""); } }

    }
}
