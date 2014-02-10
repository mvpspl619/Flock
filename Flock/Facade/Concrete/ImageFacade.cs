using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.ClientServices;
using Flock.DTO;
using Flock.DataAccess.EntityFramework;
using Flock.DataAccess.Repositories.Interfaces;
using Flock.Facade.Interfaces;
using Flock.Infrastructure.MapperProfile;

namespace Flock.Facade.Concrete
{
    public class ImageFacade : IImageFacade
    {
        private readonly IUserRepository _userRepository;
        private readonly IAutoMap _autoMap;
        

        public ImageFacade(IUserRepository userRepository, IAutoMap autoMap)
        {
            _userRepository = userRepository;
            _autoMap = autoMap;
        }

        public String ProcessImageByAction(UserImageDto img)
        {
            var result = String.Empty;
            switch (img.Action)
            {
                case "VerifyCoverPic":
                    {
                        result = VerifyImageSize(img.SourceUrl, 800, 330);
                    }
                    break;
                case "VerifyProfilePic":
                    {
                        result = VerifyProfilePicImageSize(img.SourceUrl);
                    }
                    break;
                case "PreviewCoverPic":
                    {
                        result = ImagePreview(img, 330, 880);
                    }
                    break;
               
                case "SaveCoverPic":
                    {
                        result = SaveCoverImage(img);
                    }
                    break;
                case "SaveProfilePic":
                    {
                        result = SaveProfileImage(img);
                    }
                    break;
             default:
                    return String.Empty;
            }
            return result;
        }

        private string VerifyImageSize(String imageSource, int minWidth, int minHeight)
        {
            var uploadedImg = imageSource.Substring(imageSource.IndexOf(',') + 1);
            byte[] data = Convert.FromBase64String(uploadedImg);

            Image imgPhoto = Image.FromStream(new MemoryStream(data));
            if (imgPhoto.Height < minHeight || imgPhoto.Width < minWidth)
            {
                return "false";
            }
            else
            {
                if ((imgPhoto.Width/imgPhoto.Height) > 2.666666666666667)
                {
                    return "false";
                }
                return "true";
            }
        }

        private string VerifyProfilePicImageSize(String imageSource)
        {
            var uploadedImg = imageSource.Substring(imageSource.IndexOf(',') + 1);
            byte[] data = Convert.FromBase64String(uploadedImg);

            Image imgPhoto = Image.FromStream(new MemoryStream(data));
            if (imgPhoto.Height < 64 || imgPhoto.Width < 64 || imgPhoto.Height > 400 || imgPhoto.Width > 400)
            {
                return "false";
            }
            else
            {
                return "true";
            }
        }

        public static Image ResizeImage(Image imgToResize, int width, int height)
        {
            return (Image)(new Bitmap(imgToResize, new Size(width, height)));
        }

        private string ImagePreview(UserImageDto img, int maxHeight, int maxWidth)
        {
            byte[] src;
            var currentImage = img.SourceUrl.Substring(img.SourceUrl.IndexOf(',') + 1);
            byte[] data = Convert.FromBase64String(currentImage);

            Image imgPhoto = Image.FromStream(new MemoryStream(data));
            //Resizing smaller images to minimum width ones
            imgPhoto = ResizeImage(imgPhoto, img.Width, img.Height);
            //var height = imgPhoto.Height;
            //var width = imgPhoto.Width;

            var newImage = new Bitmap(img.Width, img.Height);
            Graphics.FromImage(newImage).DrawImage(imgPhoto, 0, 0, img.Width, img.Height);
            src = CropImageFile(newImage, maxWidth, maxHeight, img.X, img.Y);
            //if (height > maxHeight && width > maxWidth)
            //{
            //    var newImage = new Bitmap(img.Width, img.Height);
            //    Graphics.FromImage(newImage).DrawImage(imgPhoto, 0, 0, img.Width, img.Height);
            //    src = CropImageFile(newImage, maxWidth, maxHeight, img.X, img.Y);
            //}
            //else if(height > maxHeight )
            //{
            //    var newImage = new Bitmap(maxWidth, maxHeight);
            //    Graphics.FromImage(newImage).DrawImage(imgPhoto, 0, 0, width, maxHeight);
            //    src = CropImageFile(newImage, img.Width, img.Height, img.X, img.Y);
            //}
            //else if(width > maxWidth )
            //{
            //    var newImage = new Bitmap(maxWidth, maxHeight);
            //    Graphics.FromImage(newImage).DrawImage(imgPhoto, 0, 0, maxWidth, height);
            //    src = CropImageFile(newImage, img.Width, img.Height, img.X, img.Y);
            //}
            //else
            //{
            //    src = CropImageFile(imgPhoto, 800, 200, img.X, img.Y);
            //}
            var imageForPreview = Convert.ToBase64String(src);
            return imageForPreview;
        }


        private byte[] CropImageFile(Image imageFile, int targetW, int targetH, int targetX, int targetY)
        {
            var imgMemoryStream = new MemoryStream();
            Image imgPhoto = imageFile;

            var bmPhoto = new Bitmap(targetW, targetH, PixelFormat.Format24bppRgb);
            bmPhoto.SetResolution(72, 72);

            Graphics grPhoto = Graphics.FromImage(bmPhoto);
            grPhoto.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            grPhoto.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
            grPhoto.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighQuality;

            try
            {
                grPhoto.DrawImage(imgPhoto, new Rectangle(0, 0, targetW, targetH),
                                    targetX, targetY, targetW, targetH, GraphicsUnit.Pixel);
                bmPhoto.Save(imgMemoryStream, ImageFormat.Jpeg);
            }

            catch (Exception e)
            {
                throw e;
            }

            return imgMemoryStream.GetBuffer();
        }

        private string SaveCoverImage(UserImageDto img)
        {
            var user = new UserDto {ID = img.UserId};
            var src = img.SourceUrl.Substring(img.SourceUrl.IndexOf(',') + 1);
            byte[] data = Convert.FromBase64String(src);
            user.CoverImage = data;
            _userRepository.UpdateUserCoverImage(_autoMap.Map<UserDto,User >(user));
            return img.SourceUrl;
        }

        private string SaveProfileImage(UserImageDto img)
        {
            var imgMemoryStream = new MemoryStream();
            var user = new UserDto { ID = img.UserId };
            var src = img.SourceUrl.Substring(img.SourceUrl.IndexOf(',') + 1);
            byte[] data = Convert.FromBase64String(src);
            
            Image imgPhoto = Image.FromStream(new MemoryStream(data));
            var newImage = new Bitmap(160, 160);
            Graphics.FromImage(newImage).DrawImage(imgPhoto, 0, 0, 160, 160);

            newImage.Save(imgMemoryStream, ImageFormat.Jpeg);
            
            byte[] modifiedImage = imgMemoryStream.GetBuffer();
            user.ProfileImage = modifiedImage;
            _userRepository.UpdateUserProfileImage(_autoMap.Map<UserDto, User>(user));
            
            return img.SourceUrl;
        }

        public byte[] GetImageFromUrl(String imageUrl)
        {
            var webClient = new WebClient();
            byte[] data = webClient.DownloadData(imageUrl);
            return data;
        }
    }
}