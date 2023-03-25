using Cysharp.Threading.Tasks;

public interface IPresenter {
    public bool isActivate { get; }
    public UniTask Open();
    public UniTask Close();
}