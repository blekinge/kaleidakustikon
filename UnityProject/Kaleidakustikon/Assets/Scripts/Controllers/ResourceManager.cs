using UnityEngine;

//This class is meant to simplify fetching resources from the file system

public class ResourceManager : MonoBehaviour {
    public static AudioClip GetAudioClip(char group, string clip, bool isMinor){
        string minor = "";
        if (isMinor){
            minor = "m";
        }
        var clippath = "Cards/Audio/" + clip + minor + "_" + TranslateGroup(group);
        //return Resources.Load<AudioClip>("Cards/" + group + "/" + clip + "sound" + minor);
        var audioClip =  Resources.Load<AudioClip>(clippath);
        Debug.Log("Loaded: " + clippath);
        return audioClip;
    }

    public static Texture GetImage(char group, string clip){
        //Debug.Log("Loading image: " + group + ", " + clip);
        return Resources.Load<Texture>("Cards/" + group + "/" + clip + "pic");
    }

    private static string TranslateGroup(char group) {
        switch (group) {
            case 'a':
                return "1";
            case 'b':
                return "2";
            case 'c':
                return "3";
			case 'd':
				return "4";
			case 'e':
				return "5";
			case 'f':
				return "6";
			case 'g':
				return "7";
			case 'h':
				return "8";
			case 'i':
				return "9";
			case 'k':
				return "10";
			case 'l':
				return "11";
			case 'm':
				return "12";
			case 'n':
				return "13";
			case 'o':
				return "14";
			case 'p':
                return "15";
            case 'q':
                return "16";
			case 'r':
				return "17";
			case 's':
				return "18";
			case 't':
				return "19";
			case 'u':
				return "20";
			case 'v':
				return "21";
        }
        return "wut";
    }
}
