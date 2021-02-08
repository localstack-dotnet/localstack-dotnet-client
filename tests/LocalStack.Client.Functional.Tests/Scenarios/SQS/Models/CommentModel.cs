using System;
using System.Collections.Generic;
using System.Text;

namespace LocalStack.Client.Functional.Tests.Scenarios.SQS.Models
{
    public class CommentModel
    {
        public Guid MovieId { get; set; }

        public string Comment { get; set; }

        public string CreateDate { get; set; }

        public Guid CommentId { get; set; }
    }
}
