//
// 스크립트 출처
// https://answers.unity.com/questions/701188/sprite-resized-to-whole-screen.html
//
using UnityEngine;

namespace Scripts.Utility
{
    [DisallowMultipleComponent]
    public class SpriteScaler : MonoBehaviour
    {
        // 참고용: https://answers.unity.com/questions/1195324/camera-size-to-show-sprite-properlly.html
        // Ortographic Size = Desired Height / (2 * pixels per unit)
        void Awake()
        {
            ResizeSpriteToScreen(gameObject, Camera.main, 1, 1);
        }

        // If fitToScreenWidth is set to 1 then the width fits the screen width.
        // If it is set to anything over 1 then the sprite will not fit the screen width, it will be divided by that number.
        // If it is set to 0 then the sprite will not resize in that dimension.
        void ResizeSpriteToScreen(GameObject theSprite, Camera theCamera, int fitToScreenWidth, int fitToScreenHeight)
        {
            SpriteRenderer sr = theSprite.GetComponent<SpriteRenderer>();

            theSprite.transform.localScale = Vector3.one;

            float width = sr.sprite.bounds.size.x;
            float height = sr.sprite.bounds.size.y;

            float worldScreenHeight = (float) (theCamera.orthographicSize * 2.0);
            float worldScreenWidth = (float) (worldScreenHeight / Screen.height * Screen.width);

            if (fitToScreenWidth != 0)
            {
                Vector2 sizeX = new Vector2(worldScreenWidth / width / fitToScreenWidth, theSprite.transform.localScale.y);
                theSprite.transform.localScale = sizeX;
            }

            if (fitToScreenHeight != 0)
            {
                Vector2 sizeY = new Vector2(theSprite.transform.localScale.x, worldScreenHeight / height / fitToScreenHeight);
                theSprite.transform.localScale = sizeY;
            }
        }
    }
}