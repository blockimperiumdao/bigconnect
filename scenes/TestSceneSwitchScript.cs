using Godot;

using System.Numerics;
using Thirdweb;

using BIGConnect.addons.godotblockchain;
using BIGConnect.addons.godotblockchain.utils;
using Nethereum.Model;

public partial class TestSceneSwitchScript : Node3D
{
	[Export]
	private Label addressLabel;	

	[Export]
	private Label tokenSymbolLabel;

	[Export]
	private Label tokenBalanceLabel;

	[Export]
	private Label tokenDropSymbolLabel;

	[Export]
	private Label tokenDropBalanceLabel;

	[Export]
	private ERC20BlockchainContractNode currencyContractNode;

	[Export]
	private ERC20BlockchainContractNode currencyDropContractNode;

	[Export]
	private ERC721BlockchainContractNode nftContractNode;

	[Export]
	private MeshInstance3D meshInstance;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		PollAddress();	

		currencyDropContractNode.ERC20BlockchainContractInitialized += OnERC20DropBlockchainContractInitialized;
		//currencyContractNode.ERC20BlockchainContractInitialized += OnERC20BlockchainContractInitialized;
		//nftContractNode.ERC721BlockchainContractInitialized += OnERC721BlockchainContractInitialized;
	}
	
	public void Log( string message )
	{
		//EmitSignal(SignalName.ClientLogMessage, "ERC721BlockchainContractNode: " + message );
		BlockchainLogManager.Instance.EmitLog("TestSceneSwitchScript: " + message);
	} 
	
	public async void OnERC20DropBlockchainContractInitialized()
	{
		Log("ERC20DropBlockchainContractInitialized");

		var metadata = await currencyContractNode.FetchMetadata();
		
		tokenDropSymbolLabel.Text = metadata.TokenSymbol;
		tokenDropBalanceLabel.Text = metadata.BalanceOf.ToString();

		if ( metadata.ClaimConditions != null )
		{
			Log("Drop Claim Condition: " + metadata.ClaimConditions.ToString());
			Log("Claiming 1.0 of currency");
			ThirdwebTransactionReceipt claimReceipt = await currencyDropContractNode.Claim("1.0");
			Log("Claimed ERC20 BlockHash: " + claimReceipt.BlockHash);
			Log("Claimed ERC20 TransactionHash: " + claimReceipt.TransactionHash);
			Log("Claimed ERC20 Status: " + claimReceipt.Status);
			
			tokenDropBalanceLabel.Text = metadata.BalanceOf.ToString();
		}
		else
		{
			Log("Metadata claim conditions are null");
		}
		
	}

	public async void OnERC20BlockchainContractInitialized()
	{
		Log("ERC20BlockchainContractInitialized");
		
		var metadata = await currencyContractNode.FetchMetadata();
		
		//balance = balance / BigInteger.Pow(10, (int)decimals);

		tokenSymbolLabel.Text = metadata.TokenSymbol;
		tokenBalanceLabel.Text = metadata.BalanceOf.ToString();
	}

	public async void OnERC721BlockchainContractInitialized()
	{
		Log("ERC721BlockchainContractInitialized");
		var nftContractMetadata = await nftContractNode.FetchMetadata();

		string tokenSymbol = nftContractMetadata.TokenSymbol;
		BigInteger tokenBalance = nftContractMetadata.BalanceOf;

		Log("NFT Symbol: " + tokenSymbol);
		Log("NFT Balance: " + tokenBalance);

		var nfts = await nftContractNode.InternalThirdwebContract.ERC721_GetAllNFTs();
		Log("NFTs owned: " + nfts.Count);

		var nftId = 0;
		Log("Getting NFT " + nftId);
		
		NFT nft = await nftContractNode.InternalThirdwebContract.ERC1155_GetNFT( nftId );

		var sprite = await TokenUtils.GetNFTAsSprite2D(nft);
		nftContractNode.AddChild(sprite);

		nftId = 1;
		
		NFT nft2 = await nftContractNode.InternalThirdwebContract.ERC1155_GetNFT( nftId );
		Log("Adding NFT to Mesh");
		var material = await TokenUtils.GetNFTAsStandardMaterial3D(nft2);
		meshInstance.Mesh.SurfaceSetMaterial(0, material);

		nftId=3;
		NFT nft3 = await nftContractNode.InternalThirdwebContract.ERC1155_GetNFT( nftId );
		Log("Pulling Audio from NFT");
		var audioStream = await TokenUtils.GetNFTAsAudioStreamMP3(nft3);
		
		var music = new AudioStreamPlayer();
		music.Stream = audioStream;
		nftContractNode.AddChild(music);
		music.Play();

		// pulls in the metadata for the NFT's claim conditions

		var myAddress = await BlockchainClientNode.Instance.smartWallet.GetAddress();

		var currencyContract = nftContractMetadata.ThirdwebCurrencyContract;
		Log("CurrencyAddress: " + nftContractMetadata.CurrencyAddress );
		Log("Price: " + nftContractMetadata.TokenPrice );

		
		// var adjusted = nftContractNode.tokenPrice / BigInteger.Pow(10, ((int)nftContractNode.currencyDecimals));

		// Log("Adjusted Price: " + adjusted );

		var approvalAmount = 6000000000000000; // this maps to .0005 of the currency since it is 18 decimals
		
		// approve the contract to spend the currency
		ThirdwebTransactionReceipt approvalReceipt = await currencyContract.ERC20_Approve(BlockchainClientNode.Instance.smartWallet, nftContractMetadata.ContractAddress, approvalAmount);
		Log("Approval ERC20 BlockHash: " + approvalReceipt.BlockHash);
		Log("Approval ERC20 TransactionHash: " + approvalReceipt.TransactionHash);
		Log("Approval ERC20 Status: " + approvalReceipt.Status);
		

		var allowance = await currencyContract.ERC20_Allowance(myAddress, nftContractMetadata.ContractAddress);
		Log("Allowance of [" + myAddress + "] for contract [" +nftContractMetadata.CurrencyAddress+ "] is [" + allowance.ToString() + "]");

		// var nativeAllowance = await currencyContract.ERC20_Allowance(myAddress, nftContractNode.contractAddress);
		// Log("Allowance of " + myAddress + " for contract " +nftContractNode.currencyAddress+ " is " + nativeAllowance.ToString());


		Log("Price Per Token [" + nftContractMetadata.TokenPrice + "] of currency [" + nftContractMetadata.CurrencyAddress + "]");
		
		var numberToClaim = 1;

		var currencyMetadata = await currencyContractNode.FetchMetadata();
		
		var amountNeeded = numberToClaim * nftContractMetadata.TokenPrice;
		if (amountNeeded > allowance)
		{
			Log("Insufficient allowance to claim NFTs");
			return;
		}
		else if (amountNeeded > currencyMetadata.BalanceOf)
		{
			Log("Insufficient funds to claim NFTs");
			return;
		}
		else
		{
			Log("Claiming " + numberToClaim + " NFTs for [" + amountNeeded + "] of [" + nftContractMetadata.CurrencyAddress + "] from a total balance of [" + currencyMetadata.BalanceOf  + "]");
			ThirdwebTransactionReceipt receipt = await nftContractNode.Claim(numberToClaim);
			Log("Claimed NFT BlockHash: " + receipt.BlockHash);
			Log("Claimed NFT TransactionHash: " + receipt.TransactionHash);
			Log("Claimed NFT Status: " + receipt.Status);
		}


	}

	public async void PollAddress()
	{
		var address = await BlockchainClientNode.Instance.smartWallet.GetAddress();
		Log("In the new scene with address " + address );
		
		addressLabel.Text = address;
	}
	
	private void OnLoginCompleted( string address )
	{
		BlockchainLogManager.Instance.EmitLog("SwitchScript address " + address);
	}

}
