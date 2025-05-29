using System;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class CustomerContrller : MonoBehaviour
{
   [SerializeField] private Transform _cameraTransform;
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

   [SerializeField] private Transform _cameraMainPos;
   [SerializeField] private Transform _cameraCutPos;
   
   [SerializeField] private Transform _spawnPos;
   [SerializeField] private Transform _centerPos;
   [SerializeField] private Transform _endPos;

   private GameObject _activeCustomer;
   private RectTransform _canvas;
   private TextMeshProUGUI _text;
   private Image _crownImage;
   private Image _ringImage;
   private Image _pendantnImage;

   private void Start()
   {
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
      
      customer.transform.DORotate(new Vector3(0, 180, 0), 0f);
      
      customer.transform.DOMove(_centerPos.position, 3f)
         .SetEase(Ease.InOutSine)
         .OnComplete(() =>
         {
            _canvas.DOScale(Vector3.one, 0.5f)
               .SetEase(Ease.InOutBack);
         });
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
