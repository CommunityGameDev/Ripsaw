using System;
using System.Xml;
using UnityEngine;

public class Core : MonoBehaviour
{


    private RipsawClient.RipsawClient _authenticationClient = new RipsawClient.RipsawClient();
    private RipsawClient.RipsawClient _loadBalancerClient = new RipsawClient.RipsawClient();
    private RipsawClient.RipsawClient _spinUpClient = new RipsawClient.RipsawClient();
    private RipsawClient.RipsawClient _gameServerClient = new RipsawClient.RipsawClient();
    private string _statusText = "";
    private UnityEngine.UI.Text _statusTextControl = null;
    private bool _statusTextChanged = false;
    private object _statusTextLock = new object();

    private UnityEngine.UI.Button _loginButton;
    private UnityEngine.UI.Button _registerButton;
    private UnityEngine.UI.InputField _emailField;
    private UnityEngine.UI.InputField _passwordField;

    private bool _isLoggedIn = false;
    GameObject mainMenu;
    bool _isLoggedInChanged = false;

    GameObject _lobbyMenu;
    private UnityEngine.UI.Button _oneVOneButton;
    private UnityEngine.UI.Button _packButton;
    private UnityEngine.UI.Button _deckButton;

    GameObject _deckList;

    long _accountID = -1;
    Guid _accountGuid = Guid.Empty;

    // Use this for initialization
    void Start()
    {

        if (_loginButton == null)
            _loginButton = GameObject.Find("loginButton").GetComponent<UnityEngine.UI.Button>();
        if (_registerButton == null)
            _registerButton = GameObject.Find("registerButton").GetComponent<UnityEngine.UI.Button>();
        if (_emailField == null)
            _emailField = GameObject.Find("emailField").GetComponent<UnityEngine.UI.InputField>();
        if (_passwordField == null)
            _passwordField = GameObject.Find("passwordField").GetComponent<UnityEngine.UI.InputField>();

        _loginButton.onClick.AddListener(Login_OnClick);
        _registerButton.onClick.AddListener(Register_OnClick);

        _lobbyMenu = GameObject.Find("lobbyMenu");
        if (_oneVOneButton == null)
            _oneVOneButton = GameObject.Find("oneVone").GetComponent<UnityEngine.UI.Button>();
        if (_packButton == null)
            _packButton = GameObject.Find("packButton").GetComponent<UnityEngine.UI.Button>();
        if (_deckButton == null)
            _deckButton = GameObject.Find("packButton").GetComponent<UnityEngine.UI.Button>();

        _oneVOneButton.onClick.AddListener(OneVOne_OnClick);
        _packButton.onClick.AddListener(Packs_OnClick);
        _deckButton.onClick.AddListener(Decks_OnClick);

        _deckList = GameObject.Find("deckList");

        if (_statusTextControl == null)
            _statusTextControl = GameObject.Find("statusText").GetComponent<UnityEngine.UI.Text>();

        _authenticationClient.OnConnectionChanged += _authenticationClient_OnConnectionChanged;
        _authenticationClient.OnIncomingDataReceived += _authenticationClient_OnIncomingDataReceived;
        _authenticationClient.Connect("ripsawstudios.ddns.net", 39450);
    }

    private void _authenticationClient_OnIncomingDataReceived(RipsawClient.RipsawClient Client, RipsawClient.DataReader DataReader, ref byte[] BufferData)
    {
        string incomingMessage = "";
        while ((incomingMessage = DataReader.IncomingStringParse(ref BufferData)) != "")
        {
            XmlDocument xml = new XmlDocument();
            xml.LoadXml(incomingMessage);
            string messageType = xml.ChildNodes[0].ChildNodes[0].InnerText;
            Debug.Log(messageType);
            switch (messageType)
            {
                case "Warning":
                    StatusTextChange(xml.ChildNodes[0].ChildNodes[1].InnerText);
                    break;
                case "Login":

                    _accountID = Convert.ToInt64(xml.ChildNodes[0].ChildNodes[1].ChildNodes[0].InnerText);
                    _accountGuid = new Guid(xml.ChildNodes[0].ChildNodes[1].ChildNodes[1].InnerText);

                    _isLoggedIn = true;
                    _isLoggedInChanged = true;
                    break;
                case "Register":
                    StatusTextChange("Account registered...");
                    break;
            }
        }
    }

    private void _authenticationClient_OnConnectionChanged(RipsawClient.RipsawClient Client, RipsawClient.eConnectionStatus ConnectionStatus)
    {
        Debug.Log(ConnectionStatus.ToString());
        StatusTextChange("Authentication Server: " + ConnectionStatus.ToString());
    }

    public void Login_OnClick()
    {
        string xmlToSend = "<RipsawMessage><MessageType>Login</MessageType><Login><Email>" + _emailField.text + "</Email><Password>" + _passwordField.text + "</Password></Login></RipsawMessage>";
        _authenticationClient.Send(xmlToSend, true);
    }

    public void Register_OnClick()
    {
        string xmlToSend = "<RipsawMessage><MessageType>Register</MessageType><Register><Email>" + _emailField.text + "</Email><Password>" + _passwordField.text + "</Password></Register></RipsawMessage>";
        _authenticationClient.Send(xmlToSend, true);
    }

    public void OneVOne_OnClick()
    {
        string xmlTest = "<RipsawMessage><MessageType>MatchMakingAdd</MessageType><Account><ID>" + _accountID.ToString() + "</ID><Guid>" + _accountGuid.ToString() + "</Guid></Account><Join><Match><Players>1v1</Players><Team></Team><Type>Default</Type></Match></Join></RipsawMessage>";
        _spinUpClient.Send(xmlTest, true);
    }

    public void Packs_OnClick()
    {

    }

    public void Decks_OnClick()
    {

    }

    private void UpdateStatusText()
    {
        _statusTextChanged = false;
        _statusTextControl.text = _statusText;
    }


    // Update is called once per frame
    void Update()
    {

        lock (_statusTextLock)
            if (_statusTextChanged)
                UpdateStatusText();

        if (_isLoggedIn)
        {
            if (_isLoggedInChanged)
            {
                _isLoggedInChanged = false;
                if (mainMenu == null)
                    mainMenu = GameObject.Find("mainMenu");
                mainMenu.SetActive(false);

                StatusTextChange("Loading list of servers...");

                _loadBalancerClient.OnConnectionChanged += _loadBalancerClient_OnConnectionChanged; ;
                _loadBalancerClient.OnIncomingDataReceived += _loadBalancerClient_OnIncomingDataReceived; ;
                _loadBalancerClient.Connect("ripsawstudios.ddns.net", 39452);

            }
        }

    }

    private void StatusTextChange(string text)
    {
        _statusText = text;
        lock (_statusTextLock)
            _statusTextChanged = true;
    }

    private void _loadBalancerClient_OnIncomingDataReceived(RipsawClient.RipsawClient Client, RipsawClient.DataReader DataReader, ref byte[] BufferData)
    {
        string incomingMessage = "";
        while ((incomingMessage = DataReader.IncomingStringParse(ref BufferData)) != "")
        {
            XmlDocument xml = new XmlDocument();
            xml.LoadXml(incomingMessage);
            string messageType = xml.ChildNodes[0].ChildNodes[0].InnerText;
            Debug.Log(messageType);
            switch (messageType)
            {
                case "Warning":
                    StatusTextChange(xml.ChildNodes[0].ChildNodes[1].InnerText);
                    break;
                case "ServerList":

                    //THIS CONTAINS ALL THE REGISTERED LOBBY/SPIN UP SERVERS --- FOR TESTING WE WILL JUST CONNECT TO THE FIRST ONE
                    XmlNodeList nodes = xml.ChildNodes[0].ChildNodes[1].ChildNodes;
                    for (int i = 0; i < nodes.Count; i++)
                    {
                        string IP = nodes[i].ChildNodes[0].InnerText;
                        int port = Convert.ToInt32(nodes[i].ChildNodes[1].InnerText);

                        //let's connect to the first one here...
                        _spinUpClient.OnConnectionChanged += _spinUpClient_OnConnectionChanged; ;
                        _spinUpClient.OnIncomingDataReceived += _spinUpClient_OnIncomingDataReceived; ;
                        _spinUpClient.Connect(IP, port);
                        break;
                    }
                    break;

            }
        }
    }

    private void _loadBalancerClient_OnConnectionChanged(RipsawClient.RipsawClient Client, RipsawClient.eConnectionStatus ConnectionStatus)
    {
        StatusTextChange("Load Balancing Server: " + ConnectionStatus.ToString());
        switch (ConnectionStatus)
        {
            case RipsawClient.eConnectionStatus.Connected:
                _loadBalancerClient.Send("<RipsawMessage><MessageType>ServerList</MessageType><Account><ID>" + _accountID.ToString() + "</ID><Guid>" + _accountGuid.ToString() + "</Guid></Account></RipsawMessage>", true);
                break;
        }
    }

    private void _spinUpClient_OnIncomingDataReceived(RipsawClient.RipsawClient Client, RipsawClient.DataReader DataReader, ref byte[] BufferData)
    {
        string incomingMessage = "";
        while ((incomingMessage = DataReader.IncomingStringParse(ref BufferData)) != "")
        {
            XmlDocument xml = new XmlDocument();
            xml.LoadXml(incomingMessage);
            string messageType = xml.ChildNodes[0].ChildNodes[0].InnerText;
            Debug.Log(messageType);
            switch (messageType)
            {
                case "CardLoaded": //this only comes for certain account types...

                    break;
                case "CardAddedToAccount":

                    break;
                case "ResourceList":
                case "SetList":
                case "RarityList":
                case "CardList":
                case "CardSaveUpdate":
                    //cardCreatorForm.ParseMessage(xml);
                    break;
                case "PackList":
                case "PackAdded":
                case "PackOpen":
                    //packForm.ParseMessage(xml);
                    break;
                case "CardListAccount":
                case "DeckSaveUpdate":
                case "DeckList":
                case "DeckLoad":
                    //deckForm.ParseMessage(xml);
                    break;
                case "Matchmaking":

                    break;
                case "MatchMakingAccountAdded":
                    Debug.Log("Account added into matchmaking...");
                    break;
                case "MatchMakingAccountRemoved":
                    //AddText("Account removed from matchmaking...");
                    break;
                case "GameCreated":
                    ////a game has been created, and we now have the ip and port of the game server we need to connect to...
                    //AddText("Game Server ready for connection... " + xml.ChildNodes[0].ChildNodes[1].ChildNodes[0].InnerText + " : " + xml.ChildNodes[0].ChildNodes[1].ChildNodes[1].InnerText);
                    _lobbyMenu.SetActive(false);
                    long _gameID = Convert.ToInt64(xml.ChildNodes[0].ChildNodes[2].ChildNodes[0].InnerText);
                    Debug.Log(_gameID);

                    ////let's kill off any previous events if they had them
                    _gameServerClient.OnIncomingDataReceived -= _gameServerClient_OnIncomingDataReceived;
                    _gameServerClient.OnConnectionChanged -= _gameServerClient_OnConnectionChanged;
                    //_gameServerClient.Disconnect();

                    ////add new events
                    //_gameServerClient.OnIncomingDataReceived += _gameServerClient_OnIncomingDataReceived;
                    //_gameServerClient.OnConnectionChanged += _gameServerClient_OnConnectionChanged;
                    //_gameServerClient.Connect(xml.ChildNodes[0].ChildNodes[1].ChildNodes[0].InnerText, Convert.ToInt32(xml.ChildNodes[0].ChildNodes[1].ChildNodes[1].InnerText));

                    break;
            }
        }
    }

    private void _spinUpClient_OnConnectionChanged(RipsawClient.RipsawClient Client, RipsawClient.eConnectionStatus ConnectionStatus)
    {
        StatusTextChange("Game Server: " + ConnectionStatus.ToString());
        switch (ConnectionStatus)
        {
            case RipsawClient.eConnectionStatus.Connected:
                _spinUpClient.Send("<RipsawMessage><MessageType>ServerConnection</MessageType><Account><ID>" + _accountID.ToString() + "</ID><Guid>" + _accountGuid.ToString() + "</Guid></Account></RipsawMessage>", true);
                _statusTextControl.gameObject.SetActive(false);
                _lobbyMenu.SetActive(true);
                break;
        }
    }

    private void _gameServerClient_OnIncomingDataReceived(RipsawClient.RipsawClient Client, RipsawClient.DataReader DataReader, ref byte[] BufferData)
    {
        string incomingMessage = "";
        while ((incomingMessage = DataReader.IncomingStringParse(ref BufferData)) != "")
        {
            XmlDocument xml = new XmlDocument();
            xml.LoadXml(incomingMessage);
            string messageType = xml.ChildNodes[0].ChildNodes[0].InnerText;
            Debug.Log(messageType);

            XmlNodeList nodeList;

            switch (messageType)
            {
                case "DeckList":
                    nodeList = xml.ChildNodes[0].ChildNodes[2].ChildNodes;
                    for (int i = 0; i < nodeList.Count; i++)
                    {
                        //listBox1.Invoke((Action)delegate { listBox1.Items.Add(nodeList[i].ChildNodes[1].InnerText + " (" + nodeList[i].ChildNodes[0].InnerText + ")"); });

                    }
                    _deckList.SetActive(true);
                    break;
                case "GameStart":

                    break;
            }
        }
    }

    private void _gameServerClient_OnConnectionChanged(RipsawClient.RipsawClient Client, RipsawClient.eConnectionStatus ConnectionStatus)
    {
        switch (ConnectionStatus)
        {
            case RipsawClient.eConnectionStatus.Failed:
                Debug.Log("Test client failed to connect to the game server... --> Retrying...");
                _gameServerClient.Connect("ripsawstudios.ddns.net", 39451);
                break;
            case RipsawClient.eConnectionStatus.Disconnected:
                Debug.Log("Test client disconnected from the game server... --> Retrying...");
                _gameServerClient.Connect("ripsawstudios.ddns.net", 39451);
                break;
            case RipsawClient.eConnectionStatus.Connecting:
                Debug.Log("Test client attempting connection to the game server...");
                break;
            case RipsawClient.eConnectionStatus.Connected:
                Debug.Log("Test client connected successfully to the game server...");
                _gameServerClient.Send("<RipsawMessage><MessageType>DeckList</MessageType><Account><ID>" + _accountID.ToString() + "</ID><Guid>" + _accountGuid.ToString() + "</Guid></Account></RipsawMessage>", true);
                //_gameForm = new GameForm(_gameServerClient, _account, _gameID);
                //this.Invoke((Action)delegate { InvokeShowGameForm(); });
                break;
        }
    }

}