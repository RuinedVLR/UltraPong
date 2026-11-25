using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TutorialPanel : MonoBehaviour
{
    [Tooltip("Each element is a page GameObject (e.g. an Image/Text group) under this panel. Only one is active at a time.")]
    [SerializeField] private List<GameObject> pages;

    [SerializeField] private Button nextButton;
    [SerializeField] private Button prevButton;
    [SerializeField] private Button skipButton;
    [SerializeField] private TMP_Text nextButtonLabel;

    private int currentIndex;
    private Action onFinished;

    public void Show(Action onFinished)
    {
        this.onFinished = onFinished;
        currentIndex = 0;

        gameObject.SetActive(true);
        RefreshPage();
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }

    public void NextPage()
    {
        if (currentIndex >= pages.Count - 1)
        {
            Finish();
            return;
        }

        currentIndex++;
        RefreshPage();
    }

    public void PreviousPage()
    {
        if (currentIndex <= 0)
            return;

        currentIndex--;
        RefreshPage();
    }

    public void Skip()
    {
        Finish();
    }

    private void Finish()
    {
        gameObject.SetActive(false);
        onFinished?.Invoke();
        onFinished = null;
    }

    private void RefreshPage()
    {
        for (int i = 0; i < pages.Count; i++)
        {
            if (pages[i] != null)
                pages[i].SetActive(i == currentIndex);
        }

        if (prevButton != null)
            prevButton.interactable = currentIndex > 0;

        bool isLastPage = currentIndex >= pages.Count - 1;

        if (nextButtonLabel != null)
            nextButtonLabel.text = isLastPage ? "Start" : "Next";
    }
}