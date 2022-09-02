import numpy

pattern = [[-1, 1], [1, 1], [1, -1], [-1, -1]]
testList = []

outfile = open('bishopPattern.txt', 'w+')
i = 0

for row in range(0,8):
    for col in range(0,8):
        output = numpy.zeros(64, dtype=int)
        
        for move in pattern:
            temp_row = row + move[0]
            temp_col = col + move[1]
            while(0 <= temp_row < 8 and 0 <= temp_col < 8):
                output[(temp_row * 8) + temp_col] = 1
                temp_row += move[0]
                temp_col += move[1]
        #  
        numberString = hex(int(''.join(str(bit) for bit in output)[::-1], 2) & 0x7e7e7e7e7e7e00) 
        testList.append(numberString)

        outfile.write(numberString)
        for x in range(18-len(numberString)):
            outfile.write(' ') 
        outfile.write(', ')

        i += 1
        if(i == 8):
            i = 0
            outfile.write('\n')

        
