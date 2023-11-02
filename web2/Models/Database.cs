using System;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.Collections.Generic;

namespace web2.Models {
    public class Database {

		public bool InsertReport(long UID, long IDToReport, int ProblemID) {
			try {
				SqlConnection cn = null;
				if (!GetDBConnection(ref cn)) throw new Exception("Database did not connect");
				SqlCommand cm = new SqlCommand("INSERT_REPORTS", cn);

				SetParameter(ref cm, "@uid", UID, SqlDbType.BigInt);
				SetParameter(ref cm, "@id_to_report", IDToReport, SqlDbType.BigInt);
				SetParameter(ref cm, "@problem_id", ProblemID, SqlDbType.TinyInt);

				SetParameter(ref cm, "ReturnValue", 0, SqlDbType.Int, Direction: ParameterDirection.ReturnValue);

				cm.ExecuteReader();

				return true;
			}
			catch (Exception ex) { throw new Exception(ex.Message); }
		}


		public int RateEvent(long UID, long ID, long Rating) {
			try {
				SqlConnection cn = null;
				if (!GetDBConnection(ref cn)) throw new Exception("Database did not connect");
				SqlCommand cm = new SqlCommand("UPDATE_EVENT_RATING", cn);
				int intReturnValue = -1;

				SetParameter(ref cm, "@rating_id", null, SqlDbType.BigInt, Direction: ParameterDirection.Output);
				SetParameter(ref cm, "@uid", UID, SqlDbType.BigInt);
				SetParameter(ref cm, "@event_id", ID, SqlDbType.BigInt);
				SetParameter(ref cm, "@rating", Rating, SqlDbType.TinyInt);

				SetParameter(ref cm, "ReturnValue", 0, SqlDbType.Int, Direction: ParameterDirection.ReturnValue);

				cm.ExecuteReader();

				//1 = new rate added
				//2 = existing rate updated
				intReturnValue = (int)cm.Parameters["ReturnValue"].Value;
				CloseDBConnection(ref cn);
				return intReturnValue;
			}
			catch (Exception ex) { throw new Exception(ex.Message); }
		}

		public List<Rating> GetEventRatings(long UID) {
			try {
				DataSet ds = new DataSet();
				SqlConnection cn = new SqlConnection();
				if (!GetDBConnection(ref cn)) throw new Exception("Database did not connect");
				SqlDataAdapter da = new SqlDataAdapter("SELECT_USER_EVENT_RATINGS", cn);
				List<Rating> ratings = new List<Rating>();

				da.SelectCommand.CommandType = CommandType.StoredProcedure;

				SetParameter(ref da, "@uid", UID, SqlDbType.BigInt);

				try {
					da.Fill(ds);
				}
				catch (Exception ex2) {
					//SysLog.UpdateLogFile(this.ToString(), MethodBase.GetCurrentMethod().Name.ToString(), ex2.Message);
				}
				finally { CloseDBConnection(ref cn); }

				if (ds.Tables[0].Rows.Count != 0) {
					foreach (DataRow dr in ds.Tables[0].Rows) {
						Rating r = new Rating();
						r.Type = Rating.Types.Event;
						r.ID = (long)dr["EventID"];
						r.Rate = (byte)dr["Rating"];
						ratings.Add(r);
					}
				}
				return ratings;
			}
			catch (Exception ex) { throw new Exception(ex.Message); }
		}

		public List<Event> GetActiveEvents() {
			try {
				DataSet ds = new DataSet();
				SqlConnection cn = new SqlConnection();
				if (!GetDBConnection(ref cn)) throw new Exception("Database did not connect");
				SqlDataAdapter da = new SqlDataAdapter("SELECT_EVENTS_ACTIVE", cn);
				List<Event> events = new List<Event>();

				da.SelectCommand.CommandType = CommandType.StoredProcedure;

				try {
					da.Fill(ds);
				}
				catch (Exception ex2) {
					//SysLog.UpdateLogFile(this.ToString(), MethodBase.GetCurrentMethod().Name.ToString(), ex2.Message);
				}
				finally { CloseDBConnection(ref cn); }

				if (ds.Tables[0].Rows.Count != 0) {
					foreach (DataRow dr in ds.Tables[0].Rows) {
						Event e = new Event();
						e.ID = (long)dr["EventID"];
						e.Title = (string)dr["Title"];
						e.Description = (string)dr["Desc"];
						if (dr["StartDate"] != null) e.Start = (DateTime)dr["StartDate"];
						if (dr["EndDate"] != null) e.End = (DateTime)dr["EndDate"];
						e.TotalLikes = (int)dr["TotalLikes"];
						if (dr["IsActive"].ToString() == "N") e.IsActive = false;
						e.AverageRating = (int)dr["AvgRating"];
						e.Location = new Location();
						e.Location.Title = (string)dr["LocationTitle"];
						e.Location.Description = (string)dr["LocationDesc"];

						e.Location.Address = new Address();
						e.Location.Address.Address1 = (string)dr["Address1"];
						e.Location.Address.Address2 = (string)dr["Address2"];
						e.Location.Address.City = (string)dr["City"];
						e.Location.Address.State = (string)dr["State"];
						e.Location.Address.Zip = (string)dr["Zip"];

						e.User = new User();
						e.User.UID = (long)dr["OwnerUID"];
						e.User.FirstName = (string)dr["FirstName"];
						e.User.LastName = (string)dr["LastName"];

						List<Image> images = GetEventImages(e.ID, 0, true);
						if (images.Count > 0) e.EventImage = images[0];

						events.Add(e);
					}
				}
				return events;
			}
			catch (Exception ex) { throw new Exception(ex.Message); }
		}

		public int ToggleEventLike(long UID, long ID) {
			try {
				SqlConnection cn = null;
				if (!GetDBConnection(ref cn)) throw new Exception("Database did not connect");
				SqlCommand cm = new SqlCommand("TOGGLE_EVENT_LIKE", cn);
				int intReturnValue = -1;

				SetParameter(ref cm, "@uid", UID, SqlDbType.BigInt);
				SetParameter(ref cm, "@event_id", ID, SqlDbType.BigInt);

				SetParameter(ref cm, "ReturnValue", 0, SqlDbType.Int, Direction: ParameterDirection.ReturnValue);

				cm.ExecuteReader();

				//1 = added
				//0 = removed
				intReturnValue = (int)cm.Parameters["ReturnValue"].Value;
				CloseDBConnection(ref cn);
				return intReturnValue;
			}
			catch (Exception ex) { throw new Exception(ex.Message); }
		}

		public List<Like> GetEventLikes(long UID) {
			try {
				DataSet ds = new DataSet();
				SqlConnection cn = new SqlConnection();
				if (!GetDBConnection(ref cn)) throw new Exception("Database did not connect");
				SqlDataAdapter da = new SqlDataAdapter("SELECT_USER_EVENT_LIKES", cn);
				List<Like> likes = new List<Like>();

				da.SelectCommand.CommandType = CommandType.StoredProcedure;

				SetParameter(ref da, "@uid", UID, SqlDbType.BigInt);

				try {
					da.Fill(ds);
				}
				catch (Exception ex2) {
					//SysLog.UpdateLogFile(this.ToString(), MethodBase.GetCurrentMethod().Name.ToString(), ex2.Message);
				}
				finally { CloseDBConnection(ref cn); }

				if (ds.Tables[0].Rows.Count != 0) {
					foreach (DataRow dr in ds.Tables[0].Rows) {
						Like l = new Like();
						l.Type = Like.Types.Event;
						l.ID = (long)dr["EventID"];
						likes.Add(l);
					}
				}
				return likes;
			}
			catch (Exception ex) { throw new Exception(ex.Message); }
		}





		public bool DeleteEvent(long ID) {
			try {
				SqlConnection cn = null;
				if (!GetDBConnection(ref cn)) throw new Exception("Database did not connect");
				SqlCommand cm = new SqlCommand("DELETE_EVENT", cn);
				int intReturnValue = -1;

				SetParameter(ref cm, "@id", ID, SqlDbType.BigInt);
				SetParameter(ref cm, "ReturnValue", 0, SqlDbType.Int, Direction: ParameterDirection.ReturnValue);

				cm.ExecuteReader();

				intReturnValue = (int)cm.Parameters["ReturnValue"].Value;
				CloseDBConnection(ref cn);

				if (intReturnValue == 1) return true;
				return false;
			}
			catch (Exception ex) { throw new Exception(ex.Message); }
		}

		public bool DeleteEventImage(long ID) {
			try {
				SqlConnection cn = null;
				if (!GetDBConnection(ref cn)) throw new Exception("Database did not connect");
				SqlCommand cm = new SqlCommand("DELETE_EVENT_IMAGE", cn);
				int intReturnValue = -1;

				SetParameter(ref cm, "@id", ID, SqlDbType.BigInt);
				SetParameter(ref cm, "ReturnValue", 0, SqlDbType.Int, Direction: ParameterDirection.ReturnValue);

				cm.ExecuteReader();

				intReturnValue = (int)cm.Parameters["ReturnValue"].Value;
				CloseDBConnection(ref cn);

				if (intReturnValue == 1) return true;
				return false;
			}
			catch (Exception ex) { throw new Exception(ex.Message); }
		}

		public Event.ActionTypes InsertEvent(Event e) {
			try {
				SqlConnection cn = null;
				if (!GetDBConnection(ref cn)) throw new Exception("Database did not connect");
				SqlCommand cm = new SqlCommand("INSERT_EVENTS", cn);
				int intReturnValue = -1;

				SetParameter(ref cm, "@id", e.ID, SqlDbType.BigInt, Direction: ParameterDirection.Output);
				SetParameter(ref cm, "@owner_uid", e.User.UID, SqlDbType.BigInt);
				SetParameter(ref cm, "@title", e.Title, SqlDbType.NVarChar);
				SetParameter(ref cm, "@desc", e.Description, SqlDbType.NVarChar);
				SetParameter(ref cm, "@start_date", e.Start, SqlDbType.DateTime);
				SetParameter(ref cm, "@end_date", e.End, SqlDbType.DateTime);
				SetParameter(ref cm, "@location_title", e.Location.Title, SqlDbType.NVarChar);
				SetParameter(ref cm, "@location_desc", e.Location.Description, SqlDbType.NVarChar);
				SetParameter(ref cm, "@address1", e.Location.Address.Address1, SqlDbType.NVarChar);
				SetParameter(ref cm, "@address2", e.Location.Address.Address2, SqlDbType.NVarChar);
				SetParameter(ref cm, "@city", e.Location.Address.City, SqlDbType.NVarChar);
				SetParameter(ref cm, "@state", e.Location.Address.State, SqlDbType.NVarChar);
				SetParameter(ref cm, "@zip", e.Location.Address.Zip, SqlDbType.NVarChar);

				if (e.IsActive)
					SetParameter(ref cm, "@is_active", "Y", SqlDbType.Char);
				else
					SetParameter(ref cm, "@is_active", "N", SqlDbType.Char);

				SetParameter(ref cm, "ReturnValue", 0, SqlDbType.TinyInt, Direction: ParameterDirection.ReturnValue);

				cm.ExecuteReader();

				intReturnValue = (int)cm.Parameters["ReturnValue"].Value;
				CloseDBConnection(ref cn);

				switch (intReturnValue) {
					case 1: // new event created
						e.ID = (long)cm.Parameters["@id"].Value;
						return Event.ActionTypes.InsertSuccessful;
					default:
						return Event.ActionTypes.Unknown;
				}
			}
			catch (Exception ex) { throw new Exception(ex.Message); }
		}

		public List<Image> GetEventImages(long EventID = 0, long EventImageID = 0, bool PrimaryOnly = false) {
			try {
				DataSet ds = new DataSet();
				SqlConnection cn = new SqlConnection();
				if (!GetDBConnection(ref cn)) throw new Exception("Database did not connect");
				SqlDataAdapter da = new SqlDataAdapter("SELECT_EVENT_IMAGES", cn);
				List<Image> imgs = new List<Image>();

				da.SelectCommand.CommandType = CommandType.StoredProcedure;

				if (EventID > 0) SetParameter(ref da, "@event_id", EventID, SqlDbType.BigInt);
				if (EventImageID > 0) SetParameter(ref da, "@event_image_id", EventImageID, SqlDbType.BigInt);
				if (PrimaryOnly) SetParameter(ref da, "@primary_only", "Y", SqlDbType.Char);

				try {
					da.Fill(ds);
				}
				catch (Exception ex2) {
					//SysLog.UpdateLogFile(this.ToString(), MethodBase.GetCurrentMethod().Name.ToString(), ex2.Message);
				}
				finally { CloseDBConnection(ref cn); }

				if (ds.Tables[0].Rows.Count != 0) {
					foreach (DataRow dr in ds.Tables[0].Rows) {
						Image i = new Image();
						i.ImageID = (long)dr["EventImageID"];
						i.ImageData = (byte[])dr["Image"];
						i.FileName = (string)dr["FileName"];
						i.Size = (long)dr["ImageSize"];
						if (dr["PrimaryImage"].ToString() == "Y")
							i.Primary = true;
						else
							i.Primary = false;
						imgs.Add(i);
					}
				}
				return imgs;
			}
			catch (Exception ex) { throw new Exception(ex.Message); }
		}

		public List<Event> GetEvents(long ID = 0, long UID = 0, string LocationTitle = "") {
			try {
				DataSet ds = new DataSet();
				SqlConnection cn = new SqlConnection();
				if (!GetDBConnection(ref cn)) throw new Exception("Database did not connect");
				SqlDataAdapter da = new SqlDataAdapter("SELECT_EVENTS", cn);
				List<Event> events = new List<Event>();

				da.SelectCommand.CommandType = CommandType.StoredProcedure;

				if (ID > 0) SetParameter(ref da, "@id", ID, SqlDbType.BigInt);
				if (UID > 0) SetParameter(ref da, "@uid", UID, SqlDbType.BigInt);
				if (LocationTitle != "") SetParameter(ref da, "@location_title", LocationTitle, SqlDbType.NVarChar);

				try {
					da.Fill(ds);
				}
				catch (Exception ex2) {
					//SysLog.UpdateLogFile(this.ToString(), MethodBase.GetCurrentMethod().Name.ToString(), ex2.Message);
				}
				finally { CloseDBConnection(ref cn); }

				if (ds.Tables[0].Rows.Count != 0) {
					foreach (DataRow dr in ds.Tables[0].Rows) {
						Event e = new Event();
						e.ID = (long)dr["EventID"];
						e.Title = (string)dr["Title"];
						e.Description = (string)dr["Desc"];
						if (dr["StartDate"] != null) e.Start = (DateTime)dr["StartDate"];
						if (dr["EndDate"] != null) e.End = (DateTime)dr["EndDate"];

						if (dr["IsActive"].ToString() == "N") e.IsActive = false;



						e.TotalLikes = (int)dr["TotalLikes"];


						e.Location = new Location();
						e.Location.Title = (string)dr["LocationTitle"];
						e.Location.Description = (string)dr["LocationDesc"];

						e.Location.Address = new Address();
						e.Location.Address.Address1 = (string)dr["Address1"];
						e.Location.Address.Address2 = (string)dr["Address2"];
						e.Location.Address.City = (string)dr["City"];
						e.Location.Address.State = (string)dr["State"];
						e.Location.Address.Zip = (string)dr["Zip"];

						e.User = new User();
						e.User.UID = (long)dr["UID"];
						e.User.UserID = (string)dr["UserID"];
						e.User.FirstName = (string)dr["FirstName"];
						e.User.LastName = (string)dr["LastName"];
						e.User.Email = (string)dr["Email"];

						List<Image> images = GetEventImages(e.ID, 0, true);
						if (images.Count > 0) e.EventImage = images[0];

						events.Add(e);
					}
				}
				return events;
			}
			catch (Exception ex) { throw new Exception(ex.Message); }
		}

		public long InsertEventImage(Event e) {
			try {
				SqlConnection cn = null;
				if (!GetDBConnection(ref cn)) throw new Exception("Database did not connect");
				SqlCommand cm = new SqlCommand("INSERT_EVENT_IMAGE", cn);

				SetParameter(ref cm, "@event_image_id", null, SqlDbType.BigInt, Direction: ParameterDirection.Output);
				SetParameter(ref cm, "@event_id", e.ID, SqlDbType.BigInt);
				if (e.EventImage.Primary)
					SetParameter(ref cm, "@primary_image", "Y", SqlDbType.Char);
				else
					SetParameter(ref cm, "@primary_image", "N", SqlDbType.Char);

				SetParameter(ref cm, "@image", e.EventImage.ImageData, SqlDbType.VarBinary);
				SetParameter(ref cm, "@file_name", e.EventImage.FileName, SqlDbType.NVarChar);
				SetParameter(ref cm, "@image_size", e.EventImage.Size, SqlDbType.BigInt);

				cm.ExecuteReader();
				CloseDBConnection(ref cn);
				return (long)cm.Parameters["@event_image_id"].Value;
			}
			catch (Exception ex) { throw new Exception(ex.Message); }
		}
		public Event.ActionTypes UpdateEvent(Event e) {
			try {
				SqlConnection cn = null;
				if (!GetDBConnection(ref cn)) throw new Exception("Database did not connect");
				SqlCommand cm = new SqlCommand("UPDATE_EVENT", cn);
				int intReturnValue = -1;

				SetParameter(ref cm, "@id", e.ID, SqlDbType.BigInt);
				SetParameter(ref cm, "@owner_uid", e.User.UID, SqlDbType.BigInt);
				SetParameter(ref cm, "@title", e.Title, SqlDbType.NVarChar);
				SetParameter(ref cm, "@desc", e.Description, SqlDbType.NVarChar);
				SetParameter(ref cm, "@start", e.Start, SqlDbType.DateTime);
				SetParameter(ref cm, "@end", e.End, SqlDbType.DateTime);
				SetParameter(ref cm, "@location_title", e.Location.Title, SqlDbType.NVarChar);
				SetParameter(ref cm, "@location_desc", e.Location.Description, SqlDbType.NVarChar);
				SetParameter(ref cm, "@address1", e.Location.Address.Address1, SqlDbType.NVarChar);
				SetParameter(ref cm, "@address2", e.Location.Address.Address2, SqlDbType.NVarChar);
				SetParameter(ref cm, "@city", e.Location.Address.City, SqlDbType.NVarChar);
				SetParameter(ref cm, "@state", e.Location.Address.State, SqlDbType.NVarChar);
				SetParameter(ref cm, "@zip", e.Location.Address.Zip, SqlDbType.NVarChar);

				if (e.IsActive)
					SetParameter(ref cm, "@is_active", "Y", SqlDbType.Char);
				else
					SetParameter(ref cm, "@is_active", "N", SqlDbType.Char);

				SetParameter(ref cm, "ReturnValue", 0, SqlDbType.Int, Direction: ParameterDirection.ReturnValue);

				cm.ExecuteReader();

				intReturnValue = (int)cm.Parameters["ReturnValue"].Value;
				CloseDBConnection(ref cn);

				switch (intReturnValue) {
					case 1: //new updated
						return Event.ActionTypes.UpdateSuccessful;
					default:
						return Event.ActionTypes.Unknown;
				}
			}
			catch (Exception ex) { throw new Exception(ex.Message); }
		}

		public long UpdateEventImage(Event e) {
			try {
				SqlConnection cn = null;
				if (!GetDBConnection(ref cn)) throw new Exception("Database did not connect");
				SqlCommand cm = new SqlCommand("UPDATE_EVENT_IMAGE", cn);

				SetParameter(ref cm, "@event_image_id", e.EventImage.ImageID, SqlDbType.BigInt);
				if (e.EventImage.Primary)
					SetParameter(ref cm, "@primary_image", "Y", SqlDbType.Char);
				else
					SetParameter(ref cm, "@primary_image", "N", SqlDbType.Char);

				SetParameter(ref cm, "@image", e.EventImage.ImageData, SqlDbType.VarBinary);
				SetParameter(ref cm, "@file_name", e.EventImage.FileName, SqlDbType.NVarChar);
				SetParameter(ref cm, "@image_size", e.EventImage.Size, SqlDbType.BigInt);

				cm.ExecuteReader();
				CloseDBConnection(ref cn);

				return 0; //success	
			}
			catch (Exception ex) { throw new Exception(ex.Message); }
		}








		public long InsertUserImage(User u) {
			try {
				SqlConnection cn = null;
				if (!GetDBConnection(ref cn)) throw new Exception("Database did not connect");
				SqlCommand cm = new SqlCommand("INSERT_USER_IMAGE", cn);

				SetParameter(ref cm, "@user_image_id", null, SqlDbType.BigInt, Direction: ParameterDirection.Output);
				SetParameter(ref cm, "@uid", u.UID, SqlDbType.BigInt);
				if (u.UserImage.Primary)
					SetParameter(ref cm, "@primary_image", "Y", SqlDbType.Char);
				else
					SetParameter(ref cm, "@primary_image", "N", SqlDbType.Char);

				SetParameter(ref cm, "@image", u.UserImage.ImageData, SqlDbType.VarBinary);
				SetParameter(ref cm, "@file_name", u.UserImage.FileName, SqlDbType.NVarChar);
				SetParameter(ref cm, "@image_size", u.UserImage.Size, SqlDbType.BigInt);

				cm.ExecuteReader();
				CloseDBConnection(ref cn);
				return (long)cm.Parameters["@user_image_id"].Value;
			}
			catch (Exception ex) { throw new Exception(ex.Message); }
		}

		public long UpdateUserImage(User u) {
			try {
				SqlConnection cn = null;
				if (!GetDBConnection(ref cn)) throw new Exception("Database did not connect");
				SqlCommand cm = new SqlCommand("UPDATE_USER_IMAGE", cn);

				SetParameter(ref cm, "@user_image_id", u.UserImage.ImageID, SqlDbType.BigInt);
				if (u.UserImage.Primary)
					SetParameter(ref cm, "@primary_image", "Y", SqlDbType.Char);
				else
					SetParameter(ref cm, "@primary_image", "N", SqlDbType.Char);

				SetParameter(ref cm, "@image", u.UserImage.ImageData, SqlDbType.VarBinary);
				SetParameter(ref cm, "@file_name", u.UserImage.FileName, SqlDbType.NVarChar);
				SetParameter(ref cm, "@image_size", u.UserImage.Size, SqlDbType.BigInt);

				cm.ExecuteReader();
				CloseDBConnection(ref cn);

				return 0; //success	
			}
			catch (Exception ex) { throw new Exception(ex.Message); }
		}

		public List<Image> GetUserImages(long UID = 0, long UserImageID = 0, bool PrimaryOnly = false) {
			try {
				DataSet ds = new DataSet();
				SqlConnection cn = new SqlConnection();
				if (!GetDBConnection(ref cn)) throw new Exception("Database did not connect");
				SqlDataAdapter da = new SqlDataAdapter("SELECT_USER_IMAGES", cn);
				List<Image> imgs = new List<Image>();

				da.SelectCommand.CommandType = CommandType.StoredProcedure;

				if (UID > 0) SetParameter(ref da, "@uid", UID, SqlDbType.BigInt);
				if (UserImageID > 0) SetParameter(ref da, "@user_image_id", UserImageID, SqlDbType.BigInt);
				if (PrimaryOnly) SetParameter(ref da, "@primary_only", "Y", SqlDbType.Char);

				try {
					da.Fill(ds);
				}
				catch (Exception ex2) {
					throw new Exception(ex2.Message);
				}
				finally { CloseDBConnection(ref cn); }

				if (ds.Tables[0].Rows.Count != 0) {
					foreach (DataRow dr in ds.Tables[0].Rows) {
						Image i = new Image();
						i.ImageID = (long)dr["UserImageID"];
						i.ImageData = (byte[])dr["Image"];
						i.FileName = (string)dr["FileName"];
						i.Size = (long)dr["ImageSize"];
						if (dr["PrimaryImage"].ToString() == "Y")
							i.Primary = true;
						else
							i.Primary = false;
						imgs.Add(i);
					}
				}
				return imgs;
			}
			catch (Exception ex) { throw new Exception(ex.Message); }
		}

		public bool DeleteUserImage(long ID) {
			try {
				SqlConnection cn = null;
				if (!GetDBConnection(ref cn)) throw new Exception("Database did not connect");
				SqlCommand cm = new SqlCommand("DELETE_USER_IMAGE", cn);
				int intReturnValue = -1;

				SetParameter(ref cm, "@id", ID, SqlDbType.BigInt);
				SetParameter(ref cm, "ReturnValue", 0, SqlDbType.Int, Direction: ParameterDirection.ReturnValue);

				cm.ExecuteReader();

				intReturnValue = (int)cm.Parameters["ReturnValue"].Value;
				CloseDBConnection(ref cn);

				if (intReturnValue == 1) return true;
				return false;

			}
			catch (Exception ex) { throw new Exception(ex.Message); }
		}



















		public User.ActionTypes InsertUser(User u) {
            try {
                SqlConnection cn = null;
                if (!GetDBConnection(ref cn)) throw new Exception("Database did not connect");
                SqlCommand cm = new SqlCommand("INSERT_USER", cn);
                int intReturnValue = -1;

                SetParameter(ref cm, "@uid", u.UID, SqlDbType.BigInt, Direction: ParameterDirection.Output);
                SetParameter(ref cm, "@user_id", u.UserID, SqlDbType.NVarChar);
                SetParameter(ref cm, "@password", u.Password, SqlDbType.NVarChar);
                SetParameter(ref cm, "@first_name", u.FirstName, SqlDbType.NVarChar);
				SetParameter(ref cm, "@last_name", u.LastName, SqlDbType.NVarChar);
                SetParameter(ref cm, "@email", u.Email, SqlDbType.NVarChar);

                SetParameter(ref cm, "ReturnValue", 0, SqlDbType.TinyInt, Direction: ParameterDirection.ReturnValue);

                cm.ExecuteReader();

                intReturnValue = (int)cm.Parameters["ReturnValue"].Value;
                CloseDBConnection(ref cn);

				switch (intReturnValue) {
                    case 1: // new user created
						u.UID = (long)cm.Parameters["@uid"].Value;
						return User.ActionTypes.InsertSuccessful;
					case -1:
						return User.ActionTypes.DuplicateEmail;
					case -2:
						return User.ActionTypes.DuplicateUserID;
                    default:
                        return User.ActionTypes.Unknown;
                }
            }
            catch (Exception ex) { throw new Exception(ex.Message); }
        }

		public User Login(User u) {
			try {
				SqlConnection cn = new SqlConnection();
				if (!GetDBConnection(ref cn)) throw new Exception("Database did not connect");
				SqlDataAdapter da = new SqlDataAdapter("LOGIN", cn);
                DataSet ds;
                User newUser = null;

                da.SelectCommand.CommandType = CommandType.StoredProcedure;

				SetParameter(ref da, "@user_id", u.UserID, SqlDbType.NVarChar);
				SetParameter(ref da, "@password", u.Password, SqlDbType.NVarChar);

				try {
					ds = new DataSet();
					da.Fill(ds);
                    if (ds.Tables[0].Rows.Count > 0) {
                        newUser = new User();
                        DataRow dr = ds.Tables[0].Rows[0];
                        newUser.UID = (long)dr["UID"];
						newUser.UserID = u.UserID;
                        newUser.Password = u.Password;
                        newUser.FirstName = (string)dr["FirstName"];
						newUser.LastName = (string)dr["LastName"];
						newUser.Email = (string)dr["Email"];						
					}
                }
				catch (Exception ex) { throw new Exception(ex.Message); }
				finally {
					CloseDBConnection(ref cn);
				}
                return newUser; //alls well in the world
			}
			catch (Exception ex) { throw new Exception(ex.Message); }
		}

		public User.ActionTypes UpdateUser(User u) {
			try {
				SqlConnection cn = null;
				if (!GetDBConnection(ref cn)) throw new Exception("Database did not connect");
				SqlCommand cm = new SqlCommand("UPDATE_USER", cn);
				int intReturnValue = -1;

				SetParameter(ref cm, "@uid", u.UID, SqlDbType.BigInt);
				SetParameter(ref cm, "@user_id", u.UserID, SqlDbType.NVarChar);
				SetParameter(ref cm, "@password", u.Password, SqlDbType.NVarChar);
				SetParameter(ref cm, "@first_name", u.FirstName, SqlDbType.NVarChar);
				SetParameter(ref cm, "@last_name", u.LastName, SqlDbType.NVarChar);
				SetParameter(ref cm, "@email", u.Email, SqlDbType.NVarChar);

				SetParameter(ref cm, "ReturnValue", 0, SqlDbType.Int, Direction: ParameterDirection.ReturnValue);

				cm.ExecuteReader();

				intReturnValue = (int)cm.Parameters["ReturnValue"].Value;
				CloseDBConnection(ref cn);

				switch (intReturnValue) {
					case 1: //new updated
						return User.ActionTypes.UpdateSuccessful;
					default:
						return User.ActionTypes.Unknown;
				}
			}
			catch (Exception ex) { throw new Exception(ex.Message); }
		}

		private bool GetDBConnection(ref SqlConnection SQLConn) {
			try {
				if (SQLConn == null) SQLConn = new SqlConnection();
				if (SQLConn.State != ConnectionState.Open) {
					SQLConn.ConnectionString = ConfigurationManager.AppSettings["AppDBConnect"];
					SQLConn.Open();
				}
				return true;
			}
			catch (Exception ex) { throw new Exception(ex.Message); }
		}

		private bool CloseDBConnection(ref SqlConnection SQLConn) {
			try {
				if (SQLConn.State != ConnectionState.Closed) {
					SQLConn.Close();
					SQLConn.Dispose();
					SQLConn = null;
				}
				return true;
			}
			catch (Exception ex) { throw new Exception(ex.Message); }
		}

		private int SetParameter(ref SqlCommand cm, string ParameterName, Object Value
			, SqlDbType ParameterType, int FieldSize = -1
			, ParameterDirection Direction = ParameterDirection.Input
			, Byte Precision = 0, Byte Scale = 0) {
			try {
				cm.CommandType = CommandType.StoredProcedure;
				if (FieldSize == -1)
					cm.Parameters.Add(ParameterName, ParameterType);
				else
					cm.Parameters.Add(ParameterName, ParameterType, FieldSize);

				if (Precision > 0) cm.Parameters[cm.Parameters.Count - 1].Precision = Precision;
				if (Scale > 0) cm.Parameters[cm.Parameters.Count - 1].Scale = Scale;

				cm.Parameters[cm.Parameters.Count - 1].Value = Value;
				cm.Parameters[cm.Parameters.Count - 1].Direction = Direction;

				return 0;
			}
			catch (Exception ex) { throw new Exception(ex.Message); }
		}

		private int SetParameter(ref SqlDataAdapter cm, string ParameterName, Object Value
			, SqlDbType ParameterType, int FieldSize = -1
			, ParameterDirection Direction = ParameterDirection.Input
			, Byte Precision = 0, Byte Scale = 0) {
			try {
				cm.SelectCommand.CommandType = CommandType.StoredProcedure;
				if (FieldSize == -1)
					cm.SelectCommand.Parameters.Add(ParameterName, ParameterType);
				else
					cm.SelectCommand.Parameters.Add(ParameterName, ParameterType, FieldSize);

				if (Precision > 0) cm.SelectCommand.Parameters[cm.SelectCommand.Parameters.Count - 1].Precision = Precision;
				if (Scale > 0) cm.SelectCommand.Parameters[cm.SelectCommand.Parameters.Count - 1].Scale = Scale;

				cm.SelectCommand.Parameters[cm.SelectCommand.Parameters.Count - 1].Value = Value;
				cm.SelectCommand.Parameters[cm.SelectCommand.Parameters.Count - 1].Direction = Direction;

				return 0;
			}
			catch (Exception ex) { throw new Exception(ex.Message); }
		}
    }
}
///////////////////////////////////////////////////////////////////////////////
//Spring 2021
///////////////////////////////////////////////////////////////////////////////
