namespace shockz.msa.ordering.application.Exceptions
{
  public class NotFoundException : ApplicationException
  {
    // id type 이 다를 수 있으므로 object key 사용
    public NotFoundException(string name, object key)
      : base($"Entity \"{name}\" {key} was not found.")
    {
    }
  }
}
