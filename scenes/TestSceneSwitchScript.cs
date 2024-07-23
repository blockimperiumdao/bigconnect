using Godot;
using NBitcoin;
using Nethereum.Model;
using System;
using System.Diagnostics.Contracts;
using System.Numerics;
using Thirdweb;
using System.IO;

public partial class TestSceneSwitchScript : Node3D
{
	[Export]
	private Label addressLabel;	

	[Export]
	private Label tokenSymbol;

	[Export]
	private Label tokenBalance;

	[Export]
	private ERC20BlockchainContractNode currencyContractNode;

	[Export]
	private ERC721BlockchainContractNode nftContractNode;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		PollAddress();	

		currencyContractNode.ERC20BlockchainContractInitialized += OnERC20BlockchainContractInitialized;
		nftContractNode.ERC721BlockchainContractInitialized += OnERC721BlockchainContractInitialized;
	}
	
	public async void OnERC20BlockchainContractInitialized()
	{
		GD.Print("ERC20BlockchainContractInitialized");

		string symbol = await currencyContractNode.Symbol();
		BigInteger balance = await currencyContractNode.BalanceOf();
		BigInteger decimals = await currencyContractNode.Decimals();

		balance = balance / BigInteger.Pow(10, (int)decimals);

		tokenSymbol.Text = symbol;
		tokenBalance.Text = balance.ToString();
	}

	public async void OnERC721BlockchainContractInitialized()
	{
		GD.Print("ERC721BlockchainContractInitialized");

		string symbol = await nftContractNode.Symbol();
		BigInteger balance = await nftContractNode.BalanceOf();

		GD.Print("NFT Symbol: " + symbol);
		GD.Print("NFT Balance: " + balance);

		var nfts = await nftContractNode.contract.ERC721_GetAllNFTs();
		GD.Print("NFTs received: " + nfts.Count);

		var nftId = 2;
		GD.Print("Getting NFT " + nftId);
		var sprite = await nftContractNode.GetNFTAsSprite2D(nftId);
		nftContractNode.AddChild(sprite);

		// try
		// {
		// 	File.WriteAllBytes("/Users/gpierce/Desktop/testnft.png", nftImageBytes);
		// }
		// catch( Exception e )
		// {
		// 	GD.Print("Exception: " + e.Message);
		// }


			//var nftImageBytes = await nfts[0].GetNFTImageBytes(BlockchainClientNode.Instance.internalClient);
		
		//GD.Print("Bytes received");
		
		// NFTMetadata nftMetadata = nft.Metadata;
		// GD.Print("ImageURL: " + nftMetadata.Image );

	}

	public async void PollAddress()
	{
		var address = await BlockchainClientNode.Instance.smartWallet.GetAddress();
		GD.Print("In the new scene with address " + address );
		
		addressLabel.Text = address;

	}
	
	private void OnLoginCompleted( string address )
	{
		BlockchainLogManager.Instance.EmitLog("SwitchScript address " + address);
		
	}

}
