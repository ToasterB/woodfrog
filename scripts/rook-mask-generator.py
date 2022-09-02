import numpy

pattern_V = [[1, 0], [-1, 0]]
pattern_H = [[0, -1], [0, 1]]
testList = []

outfile = open('rookPattern.txt', 'w+')
i = 0

for row in range(0,8):
    for col in range(0,8):
        output = numpy.zeros(64, dtype=int)
        
        for move in pattern_V:
            temp_row = row + move[0]
            while(1 <= temp_row < 7):
                output[(temp_row * 8) + col] = 1
                temp_row += move[0]

        for move in pattern_H:
            temp_col = col + move[1]
            while(1 <= temp_col < 7):
                output[(row * 8) + temp_col] = 1
                temp_col += move[1]

          
        numberString = hex(int(''.join(str(bit) for bit in output)[::-1], 2)) 
        testList.append(numberString)

        outfile.write(numberString)
        for x in range(18-len(numberString)):
            outfile.write(' ') 
        outfile.write(', ')

        i += 1
        if(i == 8):
            i = 0
            outfile.write('\n')

        
