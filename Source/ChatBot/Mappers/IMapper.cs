namespace ChatBot.Mappers;

public interface IMapper<in TInput, TResult>
{
  ICollection<TResult> Map(IEnumerable<TInput> input);
}