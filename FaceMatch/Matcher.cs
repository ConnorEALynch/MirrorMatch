using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using FaceRecognitionDotNet;


namespace FaceMatch
{
    class Matcher
    {
        private static FaceRecognition fr;
        private static string path;
        private static List<IEnumerable<FaceEncoding>> encodedFaces;
        public Matcher(string path)
        {
            if (Directory.Exists(path))
            {
                fr = FaceRecognition.Create(path);
                Matcher.path = path;
                
            }
        }
        public IEnumerable<FaceEncoding> EncodeTarget(string targetPath)
        {
            Image image = FaceRecognition.LoadImageFile(Path.Combine(path + targetPath));
            IEnumerable<Location> locations = fr.FaceLocations(image);
            IEnumerable<FaceEncoding> encoding = fr.FaceEncodings(image, locations);
            return encoding;
        }

        public IEnumerable<FaceEncoding> EncodeTarget(System.Drawing.Bitmap bitmap)
        {
            Image image = FaceRecognition.LoadImage(bitmap);
            IEnumerable<Location> locations = fr.FaceLocations(image);
            IEnumerable<FaceEncoding> encoding = fr.FaceEncodings(image, locations);
            return encoding;
        }

        public IEnumerable<FaceEncoding> EncodeTarget(byte[] bytes, int rows, int columns, int stride, Mode mode)
        {
            Image image = FaceRecognition.LoadImage(bytes, rows, columns, stride, mode);
            IEnumerable<Location> locations = fr.FaceLocations(image);
            IEnumerable<FaceEncoding> encoding = fr.FaceEncodings(image, locations);
            return encoding;
        }
        public void encodeFaces()
        {
            //encodedFaces = new IEnumerable<>();
            //string filepath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            encodedFaces = new List<IEnumerable<FaceEncoding>>();
            DirectoryInfo directory = new DirectoryInfo(path);
            var extensions = new[] { "*.png", "*.jpg" };
            var photos = extensions.SelectMany(ext => directory.GetFiles(ext));

            foreach (var photo in photos)
            {
                Image image = FaceRecognition.LoadImageFile(Path.Combine(path + @"/" + photo.Name));
                IEnumerable<Location> locations = fr.FaceLocations(image);
                IEnumerable<FaceEncoding> encoding = fr.FaceEncodings(image, locations);
                encodedFaces.Add(encoding);
            }
        }
        public IEnumerable<FaceEncoding> matchface(IEnumerable<FaceEncoding> encoding)
        {
            const double tolerance = 0.6d;
            foreach (IEnumerable<FaceEncoding> encodedFace in encodedFaces)
            {
                if (FaceRecognition.CompareFace(encoding.First(), encodedFace.First(), tolerance))
                {
                    return encodedFace;
                }
            }
            return null;
        }
    }
}
