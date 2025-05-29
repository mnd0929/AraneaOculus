using AraneaOculus.Core.Enums;

namespace AraneaOculus.Core.Models.Network
{
    public abstract class Answer : NetworkMessage
    {
        public Answer() { }

        public Answer(Request request) => Id = request.Id;

        public override NetworkCommunicationType CommunicationType => NetworkCommunicationType.Answer;

        public AnswerStatus Status { get; set; }
    }
}
