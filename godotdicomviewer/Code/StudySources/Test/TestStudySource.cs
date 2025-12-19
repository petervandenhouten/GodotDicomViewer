using FellowOakDicom.Imaging.Reconstruction;
using GodotDicomViewer.Code.Data;
using GodotDicomViewer.Code.Interfaces;
using Serilog;
using System;

namespace GodotDicomViewer.StudySources.Test
{
    public partial class TestStudySource : BaseStudySource, IStudySource
    {
        private static readonly ILogger _log = Log.ForContext<TestStudySource>();
        
        //private StudySet m_study_data;
        //private SeriesSet m_series_data;
        //private List<ImageData> m_image_data;

        public TestStudySource()
        {
            Name = "Test";
        }

        public override void _Ready()
        {
            _log.Information("TestStudySource ready");
        }

        #region IStudySource interface
        public int GetPatients(StudySourceSearchParameters search, bool force_refresh = false)
        {
            _log.Information("Called GetPatients");
            // string patient_uid = "peter";
            PatientDataAvailable?.Invoke(this.Name, [new PatientData()]);
            return new_request_count();
        }
        #endregion
        //public void Awake()
        //{
        //    var factory = new TestDataFactory();

        //    m_study_data  = factory.CreateStudyData();
        //    m_series_data = factory.CreateSeriesData();
        //    //m_image_data  = factory.CreateImageData();
        //}

        //public bool GetAllStudies(bool force_refresh)
        //{
        //    InvokeOneAndAll(m_study_data.Values.ToList());
        //    return true;
        //}

        //public bool GetSeries(string study_uid, bool force_refresh)
        //{
        //    var series = FindSeries(study_uid, m_series_data.Values);
        //    InvokeOneAndAll(study_uid, series);
        //    return true;
        //}

        ////private byte[] BitmapToByteArray(Bitmap bitmap)
        ////{
        ////    BitmapData bitmapData = bitmap.LockBits(
        ////        new Rectangle(0, 0, bitmap.Width, bitmap.Height),
        ////        ImageLockMode.ReadOnly,
        ////        bitmap.PixelFormat);

        ////    int bytesPerPixel = Bitmap.GetPixelFormatSize(bitmap.PixelFormat) / 8;
        ////    int byteCount = bitmapData.Stride * bitmap.Height;
        ////    byte[] byteArray = new byte[byteCount];

        ////    IntPtr ptrFirstPixel = bitmapData.Scan0;
        ////    System.Runtime.InteropServices.Marshal.Copy(ptrFirstPixel, byteArray, 0, byteCount);

        ////    bitmap.UnlockBits(bitmapData);
        ////    return byteArray;
        ////}

        ////public List<StudyData> GetStudies(StudySourceSearchParameters search, bool force_refresh = false)
        ////{
        ////    var result = from s in m_study_data
        ////                 where (string.IsNullOrEmpty(search.PatientName) || s.PatientName.Contains(search.PatientName, StringComparison.InvariantCultureIgnoreCase))
        ////                 //.Where(t => search.PatientID == null || t.PatientID.Contains(search.PatientID))
        ////                 //.Where(t => search.PatientName == null || t.PatientName.Contains(search.PatientName))
        ////                 select s;

        ////    return result.ToList();
        ////}

        //public bool GetImages(string study_uid, string series_uid, bool force_refresh = false)
        //{
        //    // example:
        //    string sop_uid = "example";
        //    OneImageAvailable?.Invoke(this.Name, sop_uid, new ImageData());
        //    AllImagesAvailable?.Invoke(this.Name, series_uid, new List<ImageData>());
        //    return true;
        //}

        //public bool GetStudies(StudySourceSearchParameters search, bool force_refresh)
        //{
        //    var studies = FindStudies(search, m_study_data.Values);
        //    InvokeOneAndAll(studies);
        //    return true;
        //}

        //public bool GetImage(string study_uid, string series_uid, bool force_refresh = false)
        //{
        //    return false;
        //}
    }
}
