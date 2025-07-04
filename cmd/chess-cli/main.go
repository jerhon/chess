package main

import (
	"fmt"
	chess2 "github.com/jerhon/chess/pkg/chess"
)

//TIP <p>To run your code, right-click the code and select <b>Run</b>.</p> <p>Alternatively, click
// the <icon src="AllIcons.Actions.Execute"/> icon in the gutter and select the <b>Run</b> menu item from here.</p>

func main() {
	game := chess2.NewGame()

	for {

		fmt.Println(game.GetPosition().Board.String())
		fmt.Println("Enter SAN move:")

		var input string
		_, _ = fmt.Scanln(&input)
		_, err := game.TrySanMove(input)
		if err != nil {
			println(err.Error())
		}
	}
}
