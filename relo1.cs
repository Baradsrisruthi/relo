using spproject.CommonLayer.Model;
using System.Data.SqlClient;
using System.Xml;
using System.Data;

namespace spproject.RepositoryLayer
{
    public class RegisterRL : IRegisterRL
    {
        public readonly IConfiguration _configuration;
        public readonly SqlConnection _sqlConnection;
        public RegisterRL(IConfiguration configuration)
        {
            _configuration = configuration;
            _sqlConnection = new SqlConnection(_configuration[key: "ConnectionStrings:con"]);
        }
        public async Task<CreateRecordResponse> Registration(CreateRecordRequest request)
        {
            CreateRecordResponse response = new CreateRecordResponse();
            response.IsSuccess = true;
            response.Message = "Successful";
            try
            {
                string s = "insert into Users(Name,Moblie_Number,Email,Status)values(@Name,@Moblie_Number,@Email,@Status)";
                using (SqlCommand cmd = new SqlCommand(s, _sqlConnection))
                {
                    cmd.CommandType = System.Data.CommandType.Text;
                    cmd.CommandTimeout = 180;
                    cmd.Parameters.AddWithValue(parameterName: "@Name", request.Name);
                    cmd.Parameters.AddWithValue(parameterName: "@Moblie_Number", request.Moblie_Number);
                    cmd.Parameters.AddWithValue(parameterName: "@Email", request.Email);
                    cmd.Parameters.AddWithValue(parameterName: "@Status", request.Status);
                    _sqlConnection.Open();
                    int Status = await cmd.ExecuteNonQueryAsync();
                    if(Status<=0)
                    {
                        response.IsSuccess = false;
                        response.Message = "something went wrong";
                    }
                }

            }
            catch(Exception ex)
            {
                response.IsSuccess = false;
                response.Message = ex.Message;

            }
            finally
            {
                _sqlConnection.Close();
            }
            return response;
        }

       public async Task<LoginResponse>Login(LoginRequest request)
        {
            LoginResponse response2 = new LoginResponse();
            response2.IsSuccess = true;
            response2.Message = "successful";
            

            try
            {
                _sqlConnection.Open();
                SqlDataAdapter da = new SqlDataAdapter("select * from Users Where Email='" + request.Email + "'", _sqlConnection);
                DataTable dt = new DataTable();
                da.Fill(dt);
                if (dt.Rows.Count > 0)
                {
                    string num = "0123456789";
                    int len = num.Length;
                    string OTP = string.Empty;
                    int otpdigit = 4;
                    string finaldigit;
                    int getindex;
                    for (int i = 0; i < otpdigit; i++)
                    {
                        do
                        {
                            getindex = new Random().Next(0, len);
                            finaldigit = num.ToCharArray()[getindex].ToString();
                        } while (OTP.IndexOf(finaldigit) != -1);
                        OTP += finaldigit;
                    }
                    _sqlConnection.Close();
                    string l = "";
                    _sqlConnection.Open();
                    SqlCommand userid = new SqlCommand("select * from Users  where Email='" + request.Email + "'", _sqlConnection);
                    SqlDataReader dr = userid.ExecuteReader();
                    while (dr.Read())
                    {
                        l = dr["ID"].ToString();

                    }
                    dr.Close();
                    _sqlConnection.Close();
                    _sqlConnection.Open();
                    SqlCommand cmd = new SqlCommand("insert into User_OTP(OTP, UserId ,Generted_Time,Expiry_Time)values('" + OTP + "','" + l + "',getdate(),getdate())",_sqlConnection);
                    cmd.ExecuteNonQuery();

                    
                }
            }
            catch (Exception ex)
            {
                response2.IsSuccess = false;
                response2.Message = ex.Message;
            }
            finally
            {
                _sqlConnection.Close();
            }
            return response2;
       }
    }
}
