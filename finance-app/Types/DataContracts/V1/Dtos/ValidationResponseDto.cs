using System.Collections.Generic;

namespace finance_app.Types.DataContracts.V1.Dtos {
    public class ValidationResponseDto {
        public List<ValidationError> ValidationErrors { get; set; }

    }

    public class ValidationError {
        public string Key { get; set; }
        public List<string> Errors { get; set; }

    }
}