using Microsoft.Data.SqlClient;

namespace yogabackend.Model
{
    public class Database
    {
        private readonly string _connectionString;


        public Database(string connectionString)
        {
            _connectionString = connectionString;
        }
        //Login related all the entry here 
        public List<Login> GetAllLogins()
        {
            List<Login> logins = new List<Login>();

            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                con.Open();

                string query = "SELECT login_id, username, password, active FROM Login";

                using (SqlCommand cmd = new SqlCommand(query, con))
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        logins.Add(new Login
                        {
                            LoginId = reader.GetInt32(0),
                            Username = reader.GetString(1),
                            Password = reader.GetString(2),
                            Active = reader.GetBoolean(3)
                        });
                    }
                }
            }

            return logins;
        }

        //login ends 






        // STUDENT RELATED ALL THE QUERY ARE GIVEN HERE 
        //STRICT 
        //insert the data students
        public bool InsertStudent(CreateStudentRequest student)
        {
            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                con.Open();

                // Step 1: Insert into Login and get the new login_id
                string loginQuery = @"
            INSERT INTO Login (username, password)
            VALUES (@username, @password);
            SELECT SCOPE_IDENTITY();";   // returns the last inserted identity (login_id)

                int loginId;
                using (SqlCommand loginCmd = new SqlCommand(loginQuery, con))
                {
                    loginCmd.Parameters.AddWithValue("@username", student.Username);
                    loginCmd.Parameters.AddWithValue("@password", student.Password);

                    loginId = Convert.ToInt32(loginCmd.ExecuteScalar());
                }

                // Step 2: Insert into Student using login_id
                string studentQuery = @"
            INSERT INTO Student 
            (name, current_academic_year, roll_number, dob, barcode, id_image1, id_image2, login_id)
            VALUES 
            (@name, @current_academic_year, @roll_number, @dob, @barcode, @id_image1, @id_image2, @login_id)";

                using (SqlCommand cmd = new SqlCommand(studentQuery, con))
                {
                    cmd.Parameters.AddWithValue("@name", student.Name);
                    cmd.Parameters.AddWithValue("@current_academic_year", (object)student.CurrentAcademicYear ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@roll_number", student.RollNumber);
                    cmd.Parameters.AddWithValue("@dob", (object)student.Dob ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@barcode", student.Barcode);
                    cmd.Parameters.AddWithValue("@id_image1", (object)student.IdImage1 ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@id_image2", (object)student.IdImage2 ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@login_id", loginId);

                    int rowsAffected = cmd.ExecuteNonQuery();
                    return rowsAffected > 0;
                }
            }
        }



        //get all the data students
        public List<Student> GetAllStudents()
        {
            var students = new List<Student>();

            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                con.Open();

                string query = "SELECT * FROM Student";

                using (SqlCommand cmd = new SqlCommand(query, con))
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var student = new Student
                        {
                            StudentId = Convert.ToInt32(reader["student_id"]),
                            Name = reader["name"].ToString(),
                            CurrentAcademicYear = reader["current_academic_year"]?.ToString(),
                            RollNumber = reader["roll_number"].ToString(),
                            Dob = reader["dob"] != DBNull.Value ? Convert.ToDateTime(reader["dob"]) : (DateTime?)null,
                            Barcode = reader["barcode"].ToString(),
                            IdImage1 = reader["id_image1"] != DBNull.Value ? (byte[])reader["id_image1"] : null,
                            IdImage2 = reader["id_image2"] != DBNull.Value ? (byte[])reader["id_image2"] : null,
                            LoginId = reader["login_id"] != DBNull.Value ? Convert.ToInt32(reader["login_id"]) : (int?)null
                        };

                        students.Add(student);
                    }
                }
            }

            return students;
        }


        //update the data students

        public bool UpdateStudent(int studentId, Student student)
        {
            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                con.Open();

                string query = @"
                    UPDATE Student 
SET 
    name = @name,
    current_academic_year = @current_academic_year,
    roll_number = @roll_number,
    dob = @dob,
    barcode = @barcode,
    id_image1 = @id_image1,
    id_image2 = @id_image2,
    login_id = @login_id
WHERE student_id = @student_id";

                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@student_id", studentId);
                    cmd.Parameters.AddWithValue("@name", student.Name);
                    cmd.Parameters.AddWithValue("@current_academic_year", (object)student.CurrentAcademicYear ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@roll_number", student.RollNumber);
                    cmd.Parameters.AddWithValue("@dob", (object)student.Dob ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@barcode", student.Barcode);
                    cmd.Parameters.AddWithValue("@id_image1", (object)student.IdImage1 ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@id_image2", (object)student.IdImage2 ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@login_id", (object)student.LoginId ?? DBNull.Value);

                    int rowsAffected = cmd.ExecuteNonQuery();
                    return rowsAffected > 0;
                }
            }
        }



        //get the detail on login id 
        public Student GetStudentByLoginId(int loginId)
        {
            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                con.Open();

                string query = "SELECT * FROM Student WHERE login_id = @login_id";

                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@login_id", loginId);

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new Student
                            {
                                StudentId = Convert.ToInt32(reader["student_id"]),
                                Name = reader["name"].ToString(),
                                CurrentAcademicYear = reader["current_academic_year"] as string,
                                RollNumber = reader["roll_number"].ToString(),
                                Dob = reader["dob"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(reader["dob"]),
                                Barcode = reader["barcode"].ToString(),
                                IdImage1 = reader["id_image1"] == DBNull.Value ? null : (byte[])reader["id_image1"],
                                IdImage2 = reader["id_image2"] == DBNull.Value ? null : (byte[])reader["id_image2"],
                                LoginId = reader["login_id"] == DBNull.Value ? (int?)null : Convert.ToInt32(reader["login_id"])
                            };
                        }
                    }
                }
            }

            return null; // no student found for that login_id
        }


        //ends the students 



        //THE WANDERING IMAGE RELESTED QUERY 


        //insert wandering image
        public bool InsertWanderingImage(int studentId, string description, byte[] image)
        {
            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                con.Open();

                string query = @"
                INSERT INTO WanderingImages (student_id, description, lost_thing_image, created_at)
                VALUES (@student_id, @description, @lost_thing_image, GETDATE())";

                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@student_id", studentId);
                    cmd.Parameters.AddWithValue("@description", (object)description ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@lost_thing_image", (object)image ?? DBNull.Value);

                    int rows = cmd.ExecuteNonQuery();
                    return rows > 0;
                }
            }
        }

        //  UPDATE wandering image
        public bool UpdateWanderingImage(int wanderingId, string description, byte[] image)
        {
            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                con.Open();

                string query = @"
                UPDATE WanderingImages
                SET description = @description, lost_thing_image = @lost_thing_image
                WHERE wandering_id = @wandering_id";

                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@wandering_id", wanderingId);
                    cmd.Parameters.AddWithValue("@description", (object)description ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@lost_thing_image", (object)image ?? DBNull.Value);

                    int rows = cmd.ExecuteNonQuery();
                    return rows > 0;
                }
            }
        }

        //  DELETE wandering image
        public bool DeleteWanderingImage(int wanderingId)
        {
            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                con.Open();

                string query = "DELETE FROM WanderingImages WHERE wandering_id = @wandering_id";

                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@wandering_id", wanderingId);

                    int rows = cmd.ExecuteNonQuery();
                    return rows > 0;
                }
            }
        }

        //  GET ALL wandering image
        public List<Wanderimage> GetAllWanderingImages()
        {
            List<Wanderimage> images = new List<Wanderimage>();

            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                con.Open();

                string query = "SELECT wandering_id, student_id, description, lost_thing_image, created_at FROM WanderingImages";

                using (SqlCommand cmd = new SqlCommand(query, con))
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        images.Add(new Wanderimage
                        {
                            WanderingId = reader.GetInt32(0),
                            StudentId = reader.GetInt32(1),
                            Description = reader.IsDBNull(2) ? null : reader.GetString(2),
                            LostThingImage = reader.IsDBNull(3) ? null : (byte[])reader["lost_thing_image"],
                            CreatedAt = reader.GetDateTime(4)
                        });
                    }
                }
            }

            return images;
        }



        //contact code for the details 
        // get the conteact 
        public Contact GetContactById(int contactId)
        {
            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                con.Open();

                string query = "SELECT * FROM Contact WHERE ContactID = @ContactID";

                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@ContactID", contactId);

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new Contact
                            {
                                ContactId = Convert.ToInt32(reader["ContactID"]),
                                Name = reader["Name"].ToString(),
                                Email = reader["Email"].ToString(),
                                Message = reader["Message"].ToString(),
                                CreatedAt = reader["CreatedAt"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(reader["CreatedAt"])
                            };
                        }
                    }
                }
            }

            return null; // no contact found for that ContactID
        }



        // insert the contact details

        public bool AddContact(Contact contact)
        {
            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                con.Open();

                string query = "INSERT INTO Contact (Name, Email, Message) VALUES (@Name, @Email, @Message)";

                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@Name", contact.Name);
                    cmd.Parameters.AddWithValue("@Email", contact.Email);
                    cmd.Parameters.AddWithValue("@Message", contact.Message);

                    int rowsAffected = cmd.ExecuteNonQuery();
                    return rowsAffected > 0; // true if insert succeeded
                }
            }
        }


        //delete the contact details
        public bool DeleteContact(int contactId)
        {
            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                con.Open();

                string query = "DELETE FROM Contact WHERE ContactID = @ContactID";

                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@ContactID", contactId);

                    int rowsAffected = cmd.ExecuteNonQuery();
                    return rowsAffected > 0; // true if delete succeeded
                }
            }
        }


        // this section is fro comments 
        //this add the comment 
        public bool AddComment(Contactus comment)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(_connectionString))
                {
                    con.Open();
                    string query = @"INSERT INTO [dbo].[ContactUs] 
                             (wandering_id, student_id, name, message, created_at)
                             VALUES (@WanderingId, @StudentId, @Name, @Message, @CreatedAt)";

                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@WanderingId", comment.WanderingId);
                        cmd.Parameters.AddWithValue("@StudentId", comment.StudentId);
                        cmd.Parameters.AddWithValue("@Name", comment.Name);
                        cmd.Parameters.AddWithValue("@Message", comment.Message);
                        cmd.Parameters.AddWithValue("@CreatedAt", DateTime.Now);

                        int rowsAffected = cmd.ExecuteNonQuery();
                        return rowsAffected > 0; // returns true if insert succeeded
                    }
                }
            }
            catch (Exception ex)
            {
                // optionally log ex.Message
                return false; // return false if any error occurs
            }

            // Fallback return (though it should never reach here)
            return false;
        }

        //get all the comment 
        public List<Contactus> GetAllComments()
        {
            var comments = new List<Contactus>();

            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                con.Open();
                string query = "SELECT * FROM [dbo].[ContactUs]";

                using (SqlCommand cmd = new SqlCommand(query, con))
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var comment = new Contactus
                        {
                            CommentId = Convert.ToInt32(reader["comment_id"]),
                            WanderingId = Convert.ToInt32(reader["wandering_id"]),
                            StudentId = Convert.ToInt32(reader["student_id"]),
                            Name = reader["name"].ToString(),
                            Message = reader["message"].ToString(),
                            CreatedAt = Convert.ToDateTime(reader["created_at"])
                        };

                        comments.Add(comment);
                    }
                }
            }

            return comments;
        }
        //the calendar
        public List<BookingTable> GetAllBookings()
        {
            var bookings = new List<BookingTable>();

            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                con.Open();
                string query = "SELECT * FROM [dbo].[BookingTable]";

                using (SqlCommand cmd = new SqlCommand(query, con))
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var booking = new BookingTable
                        {
                            BookingID = Convert.ToInt32(reader["BookingID"]),
                            Name = reader["Name"].ToString(),
                            DateRanges = reader["DateRanges"].ToString()
                        };

                        bookings.Add(booking);
                    }
                }
            }

            return bookings;
        }


    }
}
