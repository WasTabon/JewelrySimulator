   using System.Collections.Generic;
   using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class JewelrySet
{
   public Sprite crown;
   public Sprite ring;
   public Sprite pendant;

   public JewelrySet(Sprite crown, Sprite ring, Sprite pendant)
   {
      this.crown = crown;
      this.ring = ring;
      this.pendant = pendant;
   }
}

public class CustomerContrller : MonoBehaviour
{
   [SerializeField] private Transform _cameraTransform;
   [SerializeField] private RectTransform _createButton;
   [SerializeField] private GameObject[] _customers;
   
   [SerializeField] private Sprite _redCrownImage;
   [SerializeField] private Sprite _redRingImage;
   [SerializeField] private Sprite _redPendantImage;
   
   [SerializeField] private Sprite _blueCrownImage;
   [SerializeField] private Sprite _blueRingImage;
   [SerializeField] private Sprite _bluePendantImage;
   
   [SerializeField] private Sprite _greenCrownImage;
   [SerializeField] private Sprite _greenRingImage;
   [SerializeField] private Sprite _greenPendantImage;

   [SerializeField] private GameObject _redGem;
   [SerializeField] private GameObject _redOutline;
   [SerializeField] private GameObject _blueGem;
   [SerializeField] private GameObject _blueOutline;
   [SerializeField] private GameObject _greenGem;
   [SerializeField] private GameObject _greenOutline;

   [SerializeField] private Transform _cameraMainPos;
   [SerializeField] private Transform _cameraCutPos;
   
   [SerializeField] private Transform _spawnPos;
   [SerializeField] private Transform _centerPos;
   [SerializeField] private Transform _endPos;

   private List<JewelrySet> _sets;
   
   private GameObject _activeCustomer;
   private GameObject _currentGem;
   private RectTransform _canvas;
   private TextMeshProUGUI _text;
   private Image _crownImage;
   private Image _ringImage;
   private Image _pendantnImage;

   private void Start()
   {
      _sets = new List<JewelrySet>
      {
         new JewelrySet(_redCrownImage, _redRingImage, _redPendantImage),
         new JewelrySet(_blueCrownImage, _blueRingImage, _bluePendantImage),
         new JewelrySet(_greenCrownImage, _greenRingImage, _greenPendantImage)
      };

      _createButton.DOScale(Vector3.zero, 0f);
      
      InvokeRepeating("HandleCustomers", 0f, 3f);
   }

   private void HandleCustomers()
   {
      if (_activeCustomer != null)
         return;

      int randomCustomer = Random.Range(0, _customers.Length);

      GameObject customer = Instantiate(_customers[randomCustomer], _spawnPos.position, Quaternion.identity);
      
      _activeCustomer = customer;
      
      _canvas = FindChildByTag<RectTransform>(customer.transform, "Canvas");
      _text = FindChildByTag<TextMeshProUGUI>(customer.transform, "LeftText");
      _crownImage = FindChildByTag<Image>(customer.transform, "Crown");
      _ringImage = FindChildByTag<Image>(customer.transform, "Ring");
      _pendantnImage = FindChildByTag<Image>(customer.transform, "Pendant");

      _canvas.DOScale(Vector3.zero, 0f);

      int randomSet = Random.Range(0, _sets.Count);
      int randomItem = Random.Range(0, 3);
      JewelrySet jewelrySet = _sets[randomSet];

      switch (randomItem)
      {
         case 0:
            _crownImage.gameObject.SetActive(true);
            _crownImage.sprite = jewelrySet.crown;
            break;
         case 1:
            _ringImage.gameObject.SetActive(true);
            _ringImage.sprite = jewelrySet.ring;
            break;
         case 2:
            _pendantnImage.gameObject.SetActive(true);
            _pendantnImage.sprite = jewelrySet.pendant;
            break;
         default:
            _crownImage.gameObject.SetActive(true);
            _crownImage.sprite = jewelrySet.crown;
            break;
      }
      switch (randomSet)
      {
         case 0:
            _redGem.gameObject.SetActive(true);
            _redOutline.gameObject.SetActive(true);
            _currentGem = _redGem;
            GameState.Instance.gemType = GemType.Red;
            break;
         case 1:
            _blueGem.gameObject.SetActive(true);
            _blueOutline.gameObject.SetActive(true);
            _currentGem = _blueGem;
            GameState.Instance.gemType = GemType.Blue;
            break;
         case 2:
            _greenGem.gameObject.SetActive(true);
            _greenOutline.gameObject.SetActive(true);
            _currentGem = _greenGem;
            GameState.Instance.gemType = GemType.Green;
            break;
         default:
            _redGem.gameObject.SetActive(true);
            _redOutline.gameObject.SetActive(true);
            _currentGem = _redGem;
            GameState.Instance.gemType = GemType.Red;
            break;
      }
      
      customer.transform.DORotate(new Vector3(0, 180, 0), 0f);
      
      customer.transform.DOMove(_centerPos.position, 3f)
         .SetEase(Ease.InOutSine)
         .OnComplete(() =>
         {
            _canvas.DOScale(Vector3.one, 0.5f)
               .SetEase(Ease.InOutBack)
               .OnComplete((() =>
               {
                  _createButton.DOScale(Vector3.one, 0.5f)
                     .SetEase(Ease.InOutBack);
               }));
         });
   }

   public void HandleCreateStart()
   {
       _createButton.DOScale(Vector3.zero, 0.5f)
          .SetEase(Ease.InOutBack);
      
      Sequence sequence = DOTween.Sequence();
      
      sequence.Join(_cameraTransform.DOMove(_cameraCutPos.position, 1f)
         .SetEase(Ease.InOutSine));
      
      sequence.Join(_cameraTransform.DORotate(_cameraCutPos.eulerAngles, 1f)
         .SetEase(Ease.InOutSine));

      sequence.OnComplete((() =>
      {
         GameState.Instance.state = State.Clean;
      }));
   }
   
   private T FindChildByTag<T>(Transform parent, string tag) where T : Component
   {
      T[] components = parent.GetComponentsInChildren<T>(true);
      foreach (T component in components)
      {
         if (component.CompareTag(tag))
            return component;
      }

      Debug.LogWarning($"Не найден компонент с тегом {tag} и типом {typeof(T)} в {parent.name}");
      return null;
   }
}
