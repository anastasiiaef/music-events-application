﻿using System;

namespace web2.Models {
	public class Image {
		public long ImageID { get; set; }
		public byte[] ImageData;
		public bool Primary = false;
		public string FileName = string.Empty;
		public DateTime CreateDate;
		public long Size { get; set; }

		public string BytesBase64 {
			get {
				try {
					if (ImageData.Length > 0) { return Convert.ToBase64String(ImageData); }
					return string.Empty;
				}
				catch (Exception ex) {
					throw new Exception(ex.Message);
				}
			}
		}

		public string FileExtension {
			get {
				try {
					if (FileName == null) return string.Empty;
					return System.IO.Path.GetExtension(FileName);
				}
				catch (Exception ex) {
					throw new Exception(ex.Message);
				}
			}
		}

		public bool IsImageFile() {
			try {
				if (FileExtension.ToLower() == ".jpeg" || FileExtension.ToLower() == ".jpg" || FileExtension.ToLower() == ".bmp" || FileExtension.ToLower() == ".gif" || FileExtension.ToLower() == ".png" || FileExtension.ToLower() == ".jfif") {
					return true;
				}
				return false;
			}
			catch (Exception ex) {
				throw new Exception(ex.Message);
			}
		}
	}
}