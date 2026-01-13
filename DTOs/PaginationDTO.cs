namespace Movie_App_MinimalApi.DTOs
{
    public class PaginationDTO
    {
        public int Page { get; set; } = 1;
        private int recordsPerPage = 10; //it is default value for number of records per page.
        private readonly int maxRecordsPerPage = 50; //maximum limit for records per page.
        
        public int RecordsPerPage //purpose of it is to limit the number of records per page.
        {
            get
            {
                return recordsPerPage;
            }
            set
            {
                if (value > maxRecordsPerPage) //if user tries to set more than maximum limit then set it to maximum limit.
                {
                    recordsPerPage = maxRecordsPerPage;
                }
                else
                {
                    recordsPerPage = value;
                }
            }
        }
    }
}

//STEP TO FOLLOW TO IMPLEMENT PAGINATION IN API:
//1. Create PaginationDTO class in DTOs folder.(Done)
//GO TO ENDPOINTS FOLDER AND FOLLOW THE STEPS:
//1. In the endpoint method where you want to implement pagination, accept PUT int parameters for page number and records per page
//.so that client can specify which page they want and how many records per page.
//2. Create an instance of PaginationDTO using the received parameters.


