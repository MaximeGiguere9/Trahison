/**
 * classe TextBoxCharAnim
 * Maxime Giguère
 * 
 * TextBoxCharanim s'occupe de l'instanciation progressive des caractères dans les boîtes de texte.
 * Cela donne au texte un effet d'animation de déroulement qui supporte des modifications comme des balises de texte riche
 * pour chaque caractère individuellement. En plus, il serait possible de s'en servir pour d'autres effets comme la vibration du texte.
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TextBoxCharAnim : MonoBehaviour {

	//chaîne de texte à animer
    private string _textString;
	//position dans la chaîne
    private int _textPos;
	//dimensions de la zone de texte
	private int _lineWidth;
    private int _lineHeight;
	//liste de chaque caractère individuel affiché
    private List<GameObject> _charList;
	//indique si l'animation doit être forcée à terminée dans la même frame
    private bool _forceComplete;
	//indique si l'animation est en cours
    private bool _isWorking;
	//le prefab d'un caractère
	private GameObject _charPrefab;

	//initialiser l'animation manuellement pour s'assurer que tout est chargé
    public bool StartAnim()
    {
        //ne peut pas démarrer d'animation si elle est déjà en cours
        if (_isWorking)
        {
            Debug.LogError("Cannot start new text animation when another is in progress in the same field.");
            return false;
        }
        //vide le conteneur des caractères animés
        for(int i = 0; i < transform.childCount; i++) { 
            Destroy(transform.GetChild(i).gameObject);
        }
        //initialisation
        _forceComplete = false;
        _textPos = -1;
        _charList = new List<GameObject>();
		//dimensions
		_lineWidth = (int) gameObject.GetComponent<RectTransform> ().rect.width;
		_lineHeight = (int) (_charPrefab.GetComponent <RectTransform>().rect.height * 1.1f);
        //extrait la chaîne de texte statique placée dans la boîte par TextBoxManager
        Text textField = gameObject.transform.Find("../textField").GetComponent<Text>();
        _textString = textField.text;
        textField.text = "";
        //l'animation est en cours
        _isWorking = true;
        //démarre l'affichage progressif
        StartCoroutine(DisplayChar());
        return true;

    }

	public bool SetCharPrefab(GameObject prefab){
		_charPrefab = prefab;
		return true;
	}

	//retourne l'état actuel
    public bool IsWorking()
    {
        return _isWorking;
    }

	//force l'animation à se terminer
    public void ForceComplete()
    {
        _forceComplete = true;
    }

	//encadre un caractère par des balises de texte riche
    private string AddRichTextModifiers(string text, string richTag)
    {
        string openingTag = "<" + richTag + ">";
        //si la balise a des paramètres (e.g. color ou size), ces paramètres ne sont pas inclus dans la balise fermante
        string closingTag = "</" + richTag.Substring(0, richTag.IndexOf("=") == -1 ? richTag.Length : richTag.IndexOf("=")) + ">"; 
        return (openingTag + text + closingTag);
    }

	//ajoute une ou plusieurs balises de texte riche sur un même caractère
    private string AddRichTextModifiers(string text, List<string> richTagList)
    {
        for(int i = 0; i < richTagList.Count; i++)
        {
            text = AddRichTextModifiers(text, richTagList[i]);
        }
        return text;
    }


	//boucle de l'animation des caractères
    IEnumerator DisplayChar()
    {


		//prefabs
        GameObject charWrapper = null;
        //GameObject prefab = Resources.Load("Prefabs/textboxes/debug/charWrapper") as GameObject;
		//position sur le canvas de l'interface, par rapport au conteneur
        float xPos, yPos;
        //les caractères sont calculés en une seule ligne, les sauts se font par modulo. Cette variable contient la longueur en pixels de cette ligne (donc la position x du prochain caractère à afficher)
        float xOffset = 0;
        //considérant la variable précédente, cette variable allonge la ligne pour faire en sorte que le prochain caractère (et par conséquence tous ceux qui suivent) s'affiche sur la ligne suivante
        float endOfLineWordOffset = 0;
		//caractères
        Text letter;
		//balises de texte riche
        bool hasRichTextStyle = false;
        string richTextTag = "";
        List<string> tagList = new List<string>();

		//instancie chaque caractère avant de les afficher pour savoir ce qu'il y a à afficher
		//(note: ralentit la frame considérablement si le paragraphe est assez gros; TODO instancier un maximum de ~35 caractères par frame pour répartir le poids de l'instanciation)
        for(int i = 0; i < _textString.Length; i++)
        {
            //gestion des balises (si on en rencontre)
            if(_textString[i].ToString() == "<")
            {
                richTextTag = "";
				//les prochains caractères possèdent cette balise
                hasRichTextStyle = true;
				//extrait le nom de la balise
                for(i = i+1; i < _textString.Length; i++)
                {
					//fin de la balise
                    if (_textString[i].ToString() == ">")
                    {
						//ajoute la balise si ouvrante
                        if (hasRichTextStyle)
                        {
                            tagList.Add(richTextTag);
                        }
						//enlève la balise de la liste si fermante
                        else
                        {
                            for(int j = 0; j < tagList.Count; j++)
                            {
                                if (tagList[j].Contains(richTextTag))
                                {
                                    tagList.RemoveAt(j);
                                    break;
                                }
                            }
                        }
                        break;
                    }
					//balise fermante
                    else if (_textString[i].ToString() == "/")
                    {
						//les prochains caractères ne possèdent plus cette balise
                        hasRichTextStyle = false;
                        //"/" n'est pas ajouté au nom de la balise
                        continue;
                    }
                    richTextTag += _textString[i];
                }
                //n'instancie pas le caractère car on a affaire à une balise et non à du texte
                continue;
            }

			//instancie le caractère
			charWrapper = Instantiate(_charPrefab, gameObject.transform);        
            letter = charWrapper.GetComponent<Text>();

			//ajoute les balises de texte riches sur le caractère sauf si il est un espace
            if(_textString[i].ToString() != " ")
            {
                letter.text = AddRichTextModifiers(_textString[i].ToString(), tagList);
            }else
            {
                letter.text = _textString[i].ToString();
            }

			//positionne le caractère hors de l'écran
            charWrapper.transform.position = new Vector3(-9000, -9000, 0);
			//l'ajoute à la liste
            _charList.Add(charWrapper);
        }

        
        //affiche chaque caractère dans la boîte de texte un par un
        while (_textPos < _charList.Count)
        {
            //caractère à afficher
            _textPos++;
			//quitte si tous les caractères sont affichés
            if(_textPos == _charList.Count) { break; }
            //positionnement x du prochain caractère
            xOffset = 0;
            for(int i = 0; i < _textPos; i++)
            {
                xOffset += _charList[i].GetComponent<RectTransform>().rect.width;
            }
            xOffset += endOfLineWordOffset;
			//s'assure que les mots ne sont pas divisés
            if(_charList[_textPos].GetComponent<Text>().text == " ")
            {
                //trouve le prochain espace
                int nextSpaceIndex = _charList.FindIndex(_textPos +1, f => f.GetComponent<Text>().text == " ");
                float nextWordLineLength = 0;
                for(int i = _textPos; i < nextSpaceIndex; i++)
                {
                    nextWordLineLength += _charList[i].GetComponent<RectTransform>().rect.width;
                }
                //saute de ligne si le mot se terminerait sur la prochaine ligne
				if((nextWordLineLength + xOffset) % _lineWidth < xOffset % _lineWidth)
                {
					endOfLineWordOffset += _lineWidth - (xOffset % _lineWidth);
                }
            }
            //position du caractère
			xPos = (0 - gameObject.GetComponent<RectTransform>().rect.width / 2) + (xOffset % _lineWidth);
			yPos = (0 + gameObject.GetComponent<RectTransform>().rect.height / 2) - (_lineHeight * (Mathf.Floor(xOffset / _lineWidth)));
			_charList[_textPos].transform.localPosition = new Vector3(xPos, yPos, 0);
            //force la boucle à se terminer en une frame
            if (!_forceComplete)
            {
                yield return new WaitForSeconds(0.005f);
            }
        }
		//boucle s'est terminée, donc plus en cours
        _isWorking = false;
    }
}
